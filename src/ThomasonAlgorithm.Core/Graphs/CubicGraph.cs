namespace ThomasonAlgorithm.Core.Graphs;

/// <summary>
/// Represents a cubic (3-regular) graph, where each vertex has exactly 3 neighbors.
/// This class manages the graph structure, chord lengths, and Hamiltonian cycle.
/// </summary>
public class CubicGraph : IGraph
{
    public readonly int[,] AdjacencyMatrix;
    
    /// <summary>
    /// Gets the total number of vertices in the graph.
    /// </summary>
    public int VertexCount => AdjacencyMatrix.GetLength(0);
    
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
    public Dictionary<int, List<int>> HamiltonianCycle { get; set; }

    /// <summary>
    /// Initializes a new cubic graph with the specified number of vertices.
    /// </summary>
    /// <param name="size">The number of vertices in the graph. Must be an even number >= 4.</param>
    public CubicGraph(int size)
    {
        AdjacencyMatrix = new int[size, size];
    }
    
    public CubicGraph(int[,] adjacencyMatrix)
    {
        if (!IsCubic(adjacencyMatrix))
            throw new ArgumentOutOfRangeException(nameof(adjacencyMatrix), "Adjacency matrix is not cubic.");
        
        AdjacencyMatrix = adjacencyMatrix;
    }

    /// <summary>
    /// Adds an edge between two vertices, ensuring the graph remains cubic (3-regular).
    /// </summary>
    /// <param name="from">The starting vertex of the edge.</param>
    /// <param name="to">The ending vertex of the edge.</param>
    /// <exception cref="InvalidOperationException">Thrown if an edge already exists or the degree of a vertex exceeds 3.</exception>
    public void AddEdge(int from, int to)
    {
        ValidateVertex(from);
        ValidateVertex(to);

        if (HasEdge(from, to))
            throw new InvalidOperationException("The edge is already in the graph.");

        if (GetVertexDegree(from) >= 3 || GetVertexDegree(to) >= 3)
            throw new InvalidOperationException("Vertex degree cannot be greater than 3.");

        AdjacencyMatrix[from, to] = 1;
        AdjacencyMatrix[to, from] = 1;
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
    /// Checks if the graph is cubic, meaning each vertex has exactly 3 neighbors.
    /// </summary>
    /// <returns>True if the graph is cubic; otherwise, false.</returns>
    public bool IsCubic()
    {
        for (int i = 0; i < VertexCount; i++)
        {
            if (GetVertexDegree(i) != 3)
                return false;
        }
        return true;
    }
    
    private bool IsCubic(int[,] adjacencyMatrix)
    {
        for (var row = 0; row < adjacencyMatrix.GetLength(0); row++)
        {
            var deg = Enumerable.Range(0, adjacencyMatrix.GetLength(1))
                .Sum(colIndex => adjacencyMatrix[row, colIndex]);

            if (deg != 3)
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// Adds a chord length to the collection, increasing its count. 
    /// If the chord length doesn't exist in the collection, it will be added with an initial count of 1.
    /// Additionally, the method updates the maximum chord length if the new chord length exceeds the current maximum.
    /// </summary>
    /// <param name="chordLength">The length of the chord to be added.</param>
    /// <remarks>
    /// This method maintains a collection of chord lengths, where each chord length is mapped to its frequency (how many times it has been added).
    /// It also keeps track of the maximum chord length encountered during the process.
    /// </remarks>
    public void AddChordLength(int chordLength)
    {
        if (!ChordsLengths.ContainsKey(chordLength))
        {
            ChordsLengths.Add(chordLength, 0);
        }
        
        ChordsLengths[chordLength]++;
        
        if (chordLength > MaxChordLength)
            MaxChordLength = chordLength;
    }

    private void ValidateVertex(int vertex)
    {
        if (vertex < 0 || vertex >= VertexCount)
            throw new ArgumentOutOfRangeException(nameof(vertex), "Wrong vertex index.");
    }
}
