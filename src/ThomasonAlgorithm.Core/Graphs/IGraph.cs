namespace ThomasonAlgorithm.Core.Graphs;

public interface IGraph
{
    int VertexCount { get; }
    
    void AddEdge(int from, int to);
    void RemoveEdge(int from, int to);
    
    int GetVertexDegree(int vertex);
    List<int> GetNeighbors(int vertex);
    
    bool HasEdge(int from, int to);
}
