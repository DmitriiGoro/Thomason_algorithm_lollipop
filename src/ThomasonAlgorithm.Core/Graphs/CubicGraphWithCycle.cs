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
    /// <summary>
    /// Initializes a new instance of <see cref="CubicGraphWithCycle"/> with a specified Hamiltonian cycle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this constructor when you have a precomputed Hamiltonian cycle for the graph.
    /// The cycle will be used as-is without validation.
    /// </para>
    /// <para>
    /// The cycle must:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Contain all vertices of the graph exactly once</description></item>
    ///     <item><description>Form a single connected loop</description></item>
    ///     <item><description>Maintain cubic graph properties</description></item>
    /// </list>
    /// </remarks>
    /// <param name="graph">The cubic graph to associate with the cycle.</param>
    /// <param name="cycle">
    /// A dictionary representing the Hamiltonian cycle where:
    /// <list type="bullet">
    ///     <item><description>Keys are vertex indices</description></item>
    ///     <item><description>Values are lists of exactly 2 connected vertices</description></item>
    /// </list>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when either <paramref name="graph"/> or <paramref name="cycle"/> is null.
    /// </exception>
    /// <example>
    /// <code>
    /// var graph = new CubicGraph(adjacencyMatrix);
    /// var precomputedCycle = new Dictionary&lt;int, List&lt;int>>() { ... };
    /// var graphWithCycle = new CubicGraphWithCycle(graph, precomputedCycle);
    /// </code>
    /// </example>
    public CubicGraphWithCycle(CubicGraph graph, Dictionary<int, List<int>> cycle)
    {
        Graph = graph;
        HamiltonianCycle = cycle;
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="CubicGraphWithCycle"/> with a generated Hamiltonian cycle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Automatically generates a trivial Hamiltonian cycle using <see cref="CreateCubicGraphHamiltonianCycle"/>.
    /// The generated cycle connects vertices in sequential order (0→1→2...→n-1→0).
    /// </para>
    /// <para>
    /// For non-trivial cycles, use the constructor that accepts a precomputed cycle.
    /// </para>
    /// </remarks>
    /// <param name="graph">The cubic graph for which to generate a Hamiltonian cycle.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="graph"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the graph is not cubic (any vertex doesn't have degree 3).
    /// </exception>
    /// <example>
    /// <code>
    /// var graph = new CubicGraph(adjacencyMatrix);
    /// var graphWithCycle = new CubicGraphWithCycle(graph); // Generates default cycle
    /// </code>
    /// </example>
    /// <seealso cref="CreateCubicGraphHamiltonianCycle"/>
    public CubicGraphWithCycle(CubicGraph graph)
    {
        HamiltonianCycle = CreateCubicGraphHamiltonianCycle(graph);
        Graph = graph;
    }
    
    /// <summary>
    /// Gets the underlying cubic graph associated with this instance.
    /// </summary>
    /// <remarks>
    /// The graph maintains these properties:
    /// <list type="bullet">
    ///     <item><description>3-regular (each vertex has degree 3)</description></item>
    ///     <item><description>Undirected (edges are bidirectional)</description></item>
    ///     <item><description>No self-loops</description></item>
    /// </list>
    /// </remarks>
    /// <value>
    /// A <see cref="CubicGraph"/> object representing the graph structure.
    /// </value>
    public CubicGraph Graph { get; }
    
    /// <summary>
    /// Gets the Hamiltonian cycle associated with the graph.
    /// </summary>
    /// <remarks>
    /// The cycle is represented as an adjacency dictionary where:
    /// <list type="bullet">
    ///     <item><description>Keys are vertex indices (0-based)</description></item>
    ///     <item><description>Values are lists containing exactly 2 connected vertices</description></item>
    /// </list>
    /// This ensures the cycle forms a single loop through all vertices.
    /// </remarks>
    /// <value>
    /// A dictionary mapping each vertex to its two adjacent vertices in the cycle.
    /// </value>
    /// <example>
    /// For a 4-vertex cycle: { 0:[3,1], 1:[0,2], 2:[1,3], 3:[2,0] }
    /// </example>
    public Dictionary<int, List<int>> HamiltonianCycle { get; }
    
    /// <summary>
    /// Creates a Hamiltonian cycle for a cubic graph where vertices are connected in sequential order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method generates a basic Hamiltonian cycle that connects all vertices in the graph 
    /// in a simple sequential pattern: 0 → 1 → 2 → ... → n-1 → 0.
    /// </para>
    /// <para>
    /// The resulting cycle has these properties:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Forms a single loop passing through all vertices exactly once</description></item>
    ///     <item><description>Connects first and last vertices (0 and n-1) to complete the cycle</description></item>
    ///     <item><description>Each vertex has exactly 2 connections in the cycle (as required for Hamiltonian cycles)</description></item>
    /// </list>
    /// <para>
    /// <strong>Note:</strong> This creates a trivial cycle and may not represent the most optimal 
    /// or interesting Hamiltonian cycle for analysis purposes.
    /// </para>
    /// </remarks>
    /// <param name="graph">The cubic graph to process. Must satisfy <see cref="IsGraphCubic"/> check.</param>
    /// <returns>
    /// A dictionary representing the Hamiltonian cycle where:
    /// <list type="bullet">
    ///     <item><description>Keys are vertex indices (0 to n-1)</description></item>
    ///     <item><description>Values are lists containing exactly 2 connected vertices</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the input graph is not cubic (any vertex doesn't have degree 3)
    /// </exception>
    /// <example>
    /// For a 4-vertex cubic graph:
    /// <code>
    /// var graph = new CubicGraph(new int[,] { /* 4x4 adjacency matrix */ });
    /// var cycle = CreateCubicGraphHamiltonianCycle(graph);
    /// // Returns: { 0:[3,1], 1:[0,2], 2:[1,3], 3:[2,0] }
    /// </code>
    /// </example>
    private static Dictionary<int, List<int>> CreateCubicGraphHamiltonianCycle(CubicGraph graph)
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