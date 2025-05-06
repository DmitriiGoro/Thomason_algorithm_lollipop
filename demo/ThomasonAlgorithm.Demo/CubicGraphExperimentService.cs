using System.Diagnostics;
using System.Text;
using System.Text.Json;
using EFCore.BulkExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThomasonAlgorithm.Core.Algorithm;
using ThomasonAlgorithm.Core.GraphGenerators;
using ThomasonAlgorithm.Core.Graphs;
using ThomasonAlgorithm.Core.Models;
using ThomasonAlgorithm.Demo.Models;

namespace ThomasonAlgorithm.Demo;

/// <summary>
/// Provides functionality to run and persist experiments involving cubic graphs.
/// 
/// This service is responsible for:
/// <list type="bullet">
/// <item>Generating random cubic graphs with constraints on chord lengths.</item>
/// <item>Applying the Lollipop algorithm to detect Hamiltonian cycles.</item>
/// <item>Collecting and logging experiment data for analysis.</item>
/// <item>Storing results in a PostgreSQL database efficiently using batch inserts.</item>
/// </list>
/// </summary>
/// <remarks>
/// The service uses dependency injection to obtain <see cref="AppDbContext"/> and supports parallel graph generation for performance.
/// </remarks>
public class CubicGraphExperimentService
{
    /// <summary>
    /// Executes a cubic graph experiment cycle based on the provided parameters. 
    /// For each even vertex count from <paramref name="options.StartVerticesNumber"/> up to <paramref name="options.MaxN"/>, 
    /// the method generates a number of random cubic graphs, applies the Lollipop algorithm to find a second Hamiltonian cycle, 
    /// and stores the experiment results in the database in batches.
    /// </summary>
    /// <param name="options">The experiment configuration options, including graph size boundaries and algorithm parameters.</param>
    /// <param name="app">The application host providing access to the dependency injection container.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// The method performs the following steps:
    /// <list type="number">
    /// <item>Iterates through a range of even-sized vertex counts.</item>
    /// <item>Generates multiple cubic graphs per size, in parallel.</item>
    /// <item>Runs the Lollipop algorithm on each graph to compute the number of steps to find a second Hamiltonian cycle.</item>
    /// <item>Logs basic diagnostic information to the console.</item>
    /// <item>Stores results in the database in batches for efficiency using BulkInsert.</item>
    /// </list>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the DbContext could not be resolved.</exception>
    public async Task ExperimentCubicGraph(ExperimentOptions options, IHost app)
    {
        var (maxN, kLow, kUp, startVerticesNumber, maxAttempt, maxGraphsSameSize) = options;
        
        const int batchSize = 1000;
        const int initialStringBuilderCapacity = 256;

        var allExperiments = new List<CubicGraphExperiment>(batchSize);

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        for (var n = startVerticesNumber; n <= maxN; n += 2)
        {
            var experiments = new List<CubicGraphExperiment>(maxGraphsSameSize);
            await Parallel.ForEachAsync(Enumerable.Range(0, maxGraphsSameSize), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (graphsSameSize, ct) =>
            {
                var stringBuilder = new StringBuilder(initialStringBuilderCapacity);

                var randomCubicGraph = RandomCubicGraphGenerator.GenerateGraphLimitedChords(n, kLow, kUp, maxAttempt);

                if (randomCubicGraph == null)
                {
                    lock (Console.Out)
                    {
                        Console.WriteLine("Cubic graph was not generated.");
                    }
                    return;
                }

                var cubicGraphWithCycle = new CubicGraphWithCycle(randomCubicGraph);
                var lollipopStepsNumber = LollipopAlgorithm.FindSecondHamiltonianCycleAndReturnSteps(cubicGraphWithCycle);

                stringBuilder.AppendFormat("lollipop_steps_number: {0}, for graph with n: {1}, kLow: {2}, kUp: {3}", lollipopStepsNumber, n, kLow, kUp);
                lock (Console.Out)
                {
                    Console.WriteLine(stringBuilder.ToString());
                }
                stringBuilder.Clear();


                var experiment = new CubicGraphExperiment(n, kLow, kUp, randomCubicGraph.MaxChordLength, lollipopStepsNumber, randomCubicGraph.ChordsLengths);
                lock (experiments)
                {
                    experiments.Add(experiment);
                }
            });

            allExperiments.AddRange(experiments);

            if (allExperiments.Count >= batchSize)
            {
                await dbContext.BulkInsertAsync(allExperiments);
                allExperiments.Clear();
            }
        }

        if (allExperiments.Any())
        {
            await dbContext.BulkInsertAsync(allExperiments);
        }
    }

    public void VisualizeLollipopSteps(string graphString)
    {
        var adjacencyMatrix = GetMatrixFromNeighborsString(graphString);
        
        var cubicGraph = new CubicGraph(adjacencyMatrix);
        var cubicGraphWithCycle = new CubicGraphWithCycle(cubicGraph);
        
        LollipopAlgorithm.FindSecondHamiltonianCycleAndVisualize(cubicGraphWithCycle);
    }
    
    public void CreateLollipopStepsVisualization(string graphString)
    {
        var adjacencyMatrix = GetMatrixFromNeighborsString(graphString);
        
        var cubicGraph = new CubicGraph(adjacencyMatrix);
        var cubicGraphWithCycle = new CubicGraphWithCycle(cubicGraph);
        
        LollipopAlgorithm.CreateLollipopVisualization(cubicGraphWithCycle);
    }
    
    private int[,] GetMatrixFromNeighborsString(string neighborsString)
    {
        var neighbors = neighborsString.Split('_');
        var matrix = new int[neighbors.Length, neighbors.Length];

        for (var i = 0; i < neighbors.Length; i++)
        {
            foreach (var neib in neighbors[i].Split(','))
            {
                matrix[i, int.Parse(neib)] = 1;
            }
        }

        return matrix;
    }
}