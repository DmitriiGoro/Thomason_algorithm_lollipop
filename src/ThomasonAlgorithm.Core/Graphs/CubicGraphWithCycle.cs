namespace ThomasonAlgorithm.Core.Graphs;

/// <summary>
/// Represents a data structure that couples a cubic graph with its corresponding Hamiltonian cycle.
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
    public CubicGraph Graph { get; }
    public Dictionary<int, List<int>> HamiltonianCycle { get; }
}