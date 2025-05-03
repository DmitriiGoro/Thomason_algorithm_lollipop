using System.Text;
using EFCore.BulkExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThomasonAlgorithm.Core.Algorithm;
using ThomasonAlgorithm.Core.GraphGenerators;
using ThomasonAlgorithm.Core.Graphs;
using ThomasonAlgorithm.Demo.Models;

namespace ThomasonAlgorithm.Demo;

public class CubicGraphExperimentService
{
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
                stringBuilder.Clear();
                lock (Console.Out)
                {
                    Console.WriteLine(stringBuilder.ToString());
                }


                var experiment = new CubicGraphExperiment(n, kLow, kUp, randomCubicGraph.MaxChordLength, lollipopStepsNumber, randomCubicGraph.ChordsLengths, randomCubicGraph.AdjacencyMatrix);
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
}