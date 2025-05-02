namespace ThomasonAlgorithm.Core.Graphs;

/// <summary>
/// Represents a cubic (3-regular) graph, where each vertex has exactly 3 neighbors.
/// This class manages the graph structure, chord lengths, and Hamiltonian cycle.
/// </summary>
public class Graph : IGraph
{
    public readonly int[,] AdjacencyMatrix;
    
    /// <summary>
    /// Gets the total number of vertices in the graph.
    /// </summary>
    public int VertexCount => AdjacencyMatrix.GetLength(0);
    
    /// <summary>
    /// Initializes a new cubic graph with the specified number of vertices.
    /// </summary>
    /// <param name="size">The number of vertices in the graph. Must be an even number >= 4.</param>
    public Graph(int size)
    {
        AdjacencyMatrix = new int[size, size];
    }
    
    public Graph(int[,] adjacencyMatrix)
    {
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
            throw new InvalidOperationException("The edge is already existed.");

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
    /// Validates that a vertex index is within the valid range for the graph.
    /// </summary>
    /// <param name="vertex">The vertex index to be validated.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the vertex index is out of range.</exception>
    private void ValidateVertex(int vertex)
    {
        if (vertex < 0 || vertex >= VertexCount)
            throw new ArgumentOutOfRangeException(nameof(vertex), "Wrong vertex index.");
    }
}
