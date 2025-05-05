namespace ThomasonAlgorithm.Core.Graphs;

/// <summary>
/// Represents an unweighted, undirected graph data structure using an adjacency matrix.
/// </summary>
/// <remarks>
/// <para>
/// This class provides a foundation for graph operations using a square matrix
/// to represent vertex connections in an unweighted, undirected graph.
/// The matrix is immutable after construction.
/// </para>
/// <para>
/// Key characteristics:
/// </para>
/// <list type="bullet">
///     <item><description>Unweighted edges (values are 0 or 1)</description></item>
///     <item><description>Undirected (matrix is always symmetric)</description></item>
///     <item><description>No self-loops (diagonal elements are always 0)</description></item>
/// </list>
/// </remarks>
public class Graph
{
    /// <summary>
    /// The adjacency matrix representing connections between vertices in an unweighted, undirected graph.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Matrix characteristics:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>0 indicates no edge between vertices</description></item>
    ///     <item><description>1 indicates an existing edge (unweighted)</description></item>
    ///     <item><description>Symmetric (matrix[i,j] == matrix[j,i] for undirected graphs)</description></item>
    ///     <item><description>Zero diagonal (matrix[i,i] == 0, no self-loops)</description></item>
    /// </list>
    /// <para>
    /// This field is readonly - the matrix reference cannot be reassigned, though individual 
    /// elements can be modified through controlled methods like <see cref="AddEdge"/>.
    /// </para>
    /// </remarks>
    private readonly int[,] _adjacencyMatrix;

    /// <summary>
    /// Gets a copy of the adjacency matrix to prevent external modifications.
    /// </summary>
    public int[,] AdjacencyMatrix => (int[,])_adjacencyMatrix.Clone();

    /// <summary>
    /// Gets the total number of vertices in the graph.
    /// </summary>
    public int VertexCount => AdjacencyMatrix.GetLength(0);

    /// <summary>
    /// Initializes a new unweighted, undirected graph with the specified number of vertices.
    /// </summary>
    /// <remarks>
    /// Creates an empty adjacency matrix where:
    /// <list type="bullet">
    ///     <item><description>All elements are initialized to 0 (no edges)</description></item>
    ///     <item><description>Matrix dimensions are size × size</description></item>
    ///     <item><description>Will remain symmetric (undirected) when edges are added</description></item>
    /// </list>
    /// </remarks>
    /// <param name="size">
    /// The number of vertices in the graph. 
    /// Must be a positive integer greater than 0.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="size"/> is less than 1.
    /// </exception>
    /// <example>
    /// Create a 5-vertex graph:
    /// <code>
    /// var graph = new Graph(5); // Creates 5x5 matrix initialized with zeros
    /// </code>
    /// </example>
    public Graph(int size)
    {
        _adjacencyMatrix = new int[size, size];
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Graph"/> class using the provided adjacency matrix.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This constructor creates an unweighted, undirected graph based on the provided adjacency matrix.
    /// The matrix is cloned to prevent external modifications and ensure immutability after initialization.
    /// </para>
    /// <para>
    /// The adjacency matrix must satisfy the following conditions:
    /// <list type="bullet">
    ///     <item><description>Must be non-null</description></item>
    ///     <item><description>Must be square (n × n)</description></item>
    ///     <item><description>Must be binary (contain only 0 or 1)</description></item>
    ///     <item><description>Must be symmetric (matrix[i,j] == matrix[j,i])</description></item>
    ///     <item><description>Must have zero diagonal (no self-loops, matrix[i,i] == 0)</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <param name="adjacencyMatrix">
    /// A square, binary, symmetric matrix representing the adjacency structure of the graph.
    /// Elements are 0 (no edge) or 1 (edge exists).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="adjacencyMatrix"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="adjacencyMatrix"/> is not square, not binary, not symmetric,
    /// or contains non-zero diagonal elements.
    /// </exception>
    /// <example>
    /// Create a graph from a 3-vertex adjacency matrix:
    /// <code>
    /// int[,] matrix = new int[,]
    /// {
    ///     {0, 1, 0},
    ///     {1, 0, 1},
    ///     {0, 1, 0}
    /// };
    /// var graph = new Graph(matrix);
    /// </code>
    /// </example>
    public Graph(int[,] adjacencyMatrix)
    {
        ValidateAdjacencyMatrix(adjacencyMatrix);
        
        _adjacencyMatrix = (int[,])adjacencyMatrix.Clone();
    }
    
    /// <summary>
    /// Adds an edge between two vertices
    /// </summary>
    /// <param name="from">The starting vertex of the edge.</param>
    /// <param name="to">The ending vertex of the edge.</param>
    /// <exception cref="InvalidOperationException">Thrown if an edge already exists.</exception>
    public void AddEdge(int from, int to)
    {
        ValidateVertex(from);
        ValidateVertex(to);

        if (HasEdge(from, to))
            throw new InvalidOperationException("The edge is already in the graph.");

        _adjacencyMatrix[from, to] = 1;
        _adjacencyMatrix[to, from] = 1;
    }
    
    private void ValidateVertex(int vertex)
    {
        if (vertex < 0 || vertex >= VertexCount)
            throw new ArgumentOutOfRangeException(nameof(vertex), "Wrong vertex index.");
    }
    
    /// <summary>
    /// Checks whether an edge exists between two vertices.
    /// </summary>
    /// <param name="from">The starting vertex of the edge.</param>
    /// <param name="to">The ending vertex of the edge.</param>
    /// <returns>True if an edge exists between the two vertices; otherwise, false.</returns>
    public bool HasEdge(int from, int to)
    {
        ValidateVertex(from);
        ValidateVertex(to);
        return AdjacencyMatrix[from, to] == 1;
    }
    
    /// <summary>
    /// Removes an edge between two vertices.
    /// </summary>
    /// <param name="from">The starting vertex of the edge.</param>
    /// <param name="to">The ending vertex of the edge.</param>
    public void RemoveEdge(int from, int to)
    {
        ValidateVertex(from);
        ValidateVertex(to);

        AdjacencyMatrix[from, to] = 0;
        AdjacencyMatrix[to, from] = 0;
    }
    
    /// <summary>
    /// Gets the degree of a vertex (the number of edges connected to it).
    /// </summary>
    /// <param name="vertex">The vertex whose degree is to be calculated.</param>
    /// <returns>The degree of the specified vertex.</returns>
    public int GetVertexDegree(int vertex)
    {
        ValidateVertex(vertex);
        var degree = 0;
        for (var i = 0; i < VertexCount; i++)
            degree += AdjacencyMatrix[vertex, i];
        return degree;
    }
    
    /// <summary>
    /// Gets the list of neighbors (vertices connected by edges) for a specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex for which neighbors are to be retrieved.</param>
    /// <returns>A list of neighboring vertices.</returns>
    public List<int> GetNeighbors(int vertex)
    {
        ValidateVertex(vertex);
        var neighbors = new List<int>();
        for (int i = 0; i < VertexCount; i++)
        {
            if (AdjacencyMatrix[vertex, i] == 1)
                neighbors.Add(i);
        }
        return neighbors;
    }

    /// <summary>
    /// Determines whether the graph is regular with the specified degree for all vertices.
    /// </summary>
    /// <remarks>
    /// A graph is called <paramref name="regularity"/>-regular if every vertex has exactly 
    /// <paramref name="regularity"/> edges. Common cases include:
    /// <list type="bullet">
    ///     <item><description>3-regular (cubic) graphs</description></item>
    ///     <item><description>2-regular (cycle graphs)</description></item>
    ///     <item><description>1-regular (perfect matching graphs)</description></item>
    /// </list>
    /// </remarks>
    /// <param name="regularity">The expected degree for all vertices in the graph.</param>
    /// <returns>
    /// <c>true</c> if every vertex in the graph has degree equal to <paramref name="regularity"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// Check if a graph is cubic (3-regular):
    /// <code>
    /// if (graph.IsRegular(3)) 
    /// {
    ///     Console.WriteLine("The graph is cubic");
    /// }
    /// </code>
    /// </example>
    public bool IsRegular(int regularity)
        => Enumerable.Range(0, VertexCount).All(i => GetVertexDegree(i) == regularity);
    
    /// <summary>
    /// Determines whether the graph is cubic (3-regular).
    /// </summary>
    /// <remarks>
    /// A cubic graph is a 3-regular graph where every vertex has exactly three edges.
    /// This is equivalent to calling <see cref="IsRegular"/> with parameter 3.
    /// </remarks>
    /// <returns>
    /// <c>true</c> if the graph is cubic (all vertices have degree 3); otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// Basic usage:
    /// <code>
    /// if (graph.IsCubic()) 
    /// {
    ///     Console.WriteLine("The graph is cubic");
    /// }
    /// </code>
    /// </example>
    public bool IsCubic() 
        => IsRegular(3);

    private void ValidateAdjacencyMatrix(int[,] adjacencyMatrix)
    {
        if (adjacencyMatrix == null)
            throw new ArgumentNullException(nameof(adjacencyMatrix));

        int rows = adjacencyMatrix.GetLength(0);
        int cols = adjacencyMatrix.GetLength(1);

        // Check if matrix is square
        if (rows != cols)
            throw new ArgumentException(
                $"Adjacency matrix must be square. Got {rows}×{cols}.",
                nameof(adjacencyMatrix));

        // Check for binary values, symmetry, and zero diagonal
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int value = adjacencyMatrix[i, j];

                // Check for binary values (0 or 1)
                if (value != 0 && value != 1)
                    throw new ArgumentException(
                        $"Adjacency matrix must contain only 0 or 1. Found {value} at position [{i},{j}].",
                        nameof(adjacencyMatrix));

                // Check for zero diagonal (no self-loops)
                if (i == j && value != 0)
                    throw new ArgumentException(
                        $"Adjacency matrix must have zero diagonal (no self-loops). Found {value} at position [{i},{i}].",
                        nameof(adjacencyMatrix));

                // Check for symmetry (undirected graph)
                if (adjacencyMatrix[i, j] != adjacencyMatrix[j, i])
                    throw new ArgumentException(
                        $"Adjacency matrix must be symmetric. Found matrix[{i},{j}]={adjacencyMatrix[i, j]} " +
                        $"but matrix[{j},{i}]={adjacencyMatrix[j, i]}.",
                        nameof(adjacencyMatrix));
            }
        }
    }
}
