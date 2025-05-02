using ThomasonAlgorithm.Core.Graphs;

namespace ThomasonAlgorithm.Core.Utils;

public static class HamiltonianCycleBuilder
{
    /// <summary>
    /// Generates a Hamiltonian cycle for a cubic graph with <paramref name="n"/> vertices.
    /// The cycle visits all vertices from 0 to <paramref name="n"/> - 1 in sequential order, 
    /// with each vertex connected to its adjacent vertices in the cycle. 
    /// The cycle is represented as an undirected graph where each vertex is connected to its
    /// predecessor and successor in the cycle.
    /// </summary>
    /// <param name="n">The number of vertices in the graph. Must be a positive integer (n ≥ 2).</param>
    /// <param name="graph">cubicGraph</param>
    /// <returns>
    /// A dictionary where the key is a vertex index (0 to <paramref name="n"/> - 1), 
    /// and the value is a list of neighboring vertices in the Hamiltonian cycle. 
    /// Each vertex will have exactly two neighbors, one before and one after it in the cycle.
    /// </returns>
    /// <remarks>
    /// This method generates a simple Hamiltonian cycle where each vertex is connected
    /// to the next one in sequence, with the first and last vertices connected to form a loop.
    /// The result is useful for constructing cubic graphs where the Hamiltonian cycle is needed
    /// for further graph manipulations or as a base for other algorithms.
    /// </remarks>
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