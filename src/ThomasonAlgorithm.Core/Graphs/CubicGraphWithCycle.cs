namespace ThomasonAlgorithm.Core.Graphs;

/// <summary>
/// Represents a cubic graph with an associated Hamiltonian cycle. 
/// Provides functionality to initialize the graph with or without a provided cycle.
/// </summary>
/// <remarks>
/// This class ensures that a Hamiltonian cycle is explicitly associated with a specific cubic graph,
/// providing better consistency and clarity when passing this data between methods or components.
/// </remarks>
public class CubicGraphWithCycle
{
    public CubicGraphWithCycle(CubicGraph graph, Dictionary<int, List<int>> cycle)
    {
        Graph = graph;
        HamiltonianCycle = cycle;
    }
    
    public CubicGraphWithCycle(CubicGraph graph)
    {
        Graph = graph;
        HamiltonianCycle = CreateCubicGraphHamiltonianCycle(graph);
    }
    
    public CubicGraph Graph { get; }
    
    public Dictionary<int, List<int>> HamiltonianCycle { get; }
    
    public static Dictionary<int, List<int>> CreateCubicGraphHamiltonianCycle(CubicGraph graph)
    {
        if (!IsGraphCubic(graph))
            throw new ArgumentOutOfRangeException(nameof(graph), "The given graph is not cubic.");
        
        var n = graph.VertexCount;

        var cycle = Enumerable
            .Range(0, n)
            .ToDictionary(i => i, i => new List<int>());

        cycle[0].Add(n - 1);
        cycle[n - 1].Add(0);

        for (var i = 1; i < n; i++)
        {
            cycle[i].Add(i - 1);
            cycle[i - 1].Add(i);
        }

        return cycle;
    }
    
    private static bool IsGraphCubic(CubicGraph graph)
    {
        for (var row = 0; row < graph.AdjacencyMatrix.GetLength(0); row++)
        {
            var deg = Enumerable.Range(0, graph.AdjacencyMatrix.GetLength(1))
                .Sum(colIndex => graph.AdjacencyMatrix[row, colIndex]);

            if (deg != 3)
                return false;
        }
        return true;
    }
}