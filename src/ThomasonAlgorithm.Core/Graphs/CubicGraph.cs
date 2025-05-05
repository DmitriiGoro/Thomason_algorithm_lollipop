namespace ThomasonAlgorithm.Core.Graphs;

/// <summary>
/// Represents a cubic (3-regular) graph, where each vertex has exactly 3 neighbors.
/// This class manages the graph structure, chord lengths, and Hamiltonian cycle.
/// </summary>
public class CubicGraph : Graph
{
    /// <summary>
    /// Gets the adjacency matrix of the cubic graph.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This matrix has the following invariants:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Exactly three non-zero entries per row/column (3-regular)</description></item>
    ///     <item><description>Symmetric (matrix[i,j] == matrix[j,i] for undirected graphs)</description></item>
    ///     <item><description>Zero diagonal (no self-loops)</description></item>
    /// </list>
    /// <para>
    /// This property hides the base <see cref="Graph.AdjacencyMatrix"/> to enforce cubic graph constraints.
    /// </para>
    /// </remarks>
    /// <value>
    /// A square 2D integer array representing vertex connections where:
    /// <list type="bullet">
    ///     <item><description>1 indicates an edge between vertices</description></item>
    ///     <item><description>0 indicates no connection</description></item>
    /// </list>
    /// </value>
    /// <example>
    /// Example for a 4-vertex cube graph:
    /// <code>
    /// [
    ///     [0, 1, 1, 1],
    ///     [1, 0, 1, 1],
    ///     [1, 1, 0, 1],
    ///     [1, 1, 1, 0]
    /// ]
    /// </code>
    /// </example>
    private readonly int[,] _adjacencyMatrix;
    
    /// <summary>
    /// Gets a copy of the adjacency matrix to prevent external modifications.
    /// </summary>
    public new int[,] AdjacencyMatrix => (int[,])_adjacencyMatrix.Clone();
    
    /// <summary>
    /// Gets or sets the maximum chord length found in the graph.
    /// </summary>
    public int MaxChordLength = -1;
    
    /// <summary>
    /// Dictionary holding the chord lengths and the number of times each chord length occurs.
    /// </summary>
    public readonly Dictionary<int, int> ChordsLengths = new Dictionary<int, int>();
    
    /// <summary>
    /// Stores the Hamiltonian cycle of the graph, represented as a dictionary of vertices and their neighbors in the cycle.
    /// </summary>
    public Dictionary<int, List<int>>? HamiltonianCycle { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CubicGraph"/> class using the provided adjacency matrix.
    /// </summary>
    /// <param name="adjacencyMatrix">A square matrix representing the adjacency structure of the graph.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the provided adjacency matrix does not correspond to a valid cubic graph,
    /// i.e., each vertex must have exactly three edges.
    /// </exception>
    /// <remarks>
    /// This constructor ensures that the input graph is cubic at the time of initialization. 
    /// It also computes the chord lengths for all applicable node pairs and sets the maximum chord length
    /// found in the graph.
    /// </remarks>
    public CubicGraph(int[,] adjacencyMatrix) : base(adjacencyMatrix)
    {
        if (!IsCubic(adjacencyMatrix))
            throw new ArgumentOutOfRangeException(nameof(adjacencyMatrix), "Adjacency matrix is not cubic.");
        
        _adjacencyMatrix = (int[,])adjacencyMatrix.Clone(); 
        ComputeChordLengths();
    }

    /// <summary>
    /// Adds an edge between two vertices, ensuring the graph remains cubic (3-regular).
    /// </summary>
    /// <param name="from">The starting vertex of the edge.</param>
    /// <param name="to">The ending vertex of the edge.</param>
    /// <exception cref="InvalidOperationException">Thrown if an edge already exists or the degree of a vertex exceeds 3.</exception>
    public new void AddEdge(int from, int to)
    {
        if (GetVertexDegree(from) >= 3 || GetVertexDegree(to) >= 3)
            throw new InvalidOperationException("Vertex degree cannot be greater than 3.");
        base.AddEdge(from, to);
    }
    
    private bool IsCubic(int[,] adjacencyMatrix)
    {
        if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
            throw new ArgumentException(
                $"Invalid adjacency matrix dimensions. Expected square matrix (n×n), " +
                $"got {adjacencyMatrix.GetLength(0)}×{adjacencyMatrix.GetLength(1)}.");
        
        for (var row = 0; row < adjacencyMatrix.GetLength(0); row++)
        {
            var deg = Enumerable.Range(0, adjacencyMatrix.GetLength(1))
                .Sum(colIndex => adjacencyMatrix[row, colIndex]);

            if (deg != 3)
                return false;
        }
        return true;
    }
    
    private void ComputeChordLengths()
    {
        var n = _adjacencyMatrix.GetLength(0);

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (_adjacencyMatrix[i, j] != 1) 
                    continue;
                
                var chordLength = Math.Min(Math.Abs(i - j), n - Math.Abs(i - j));
                
                if (chordLength <= 1)
                    continue;

                if (!ChordsLengths.TryAdd(chordLength, 1))
                {
                    ChordsLengths[chordLength]++;
                }

                if (chordLength > MaxChordLength)
                {
                    MaxChordLength = chordLength;
                }
            }
        }
    }

}
