namespace ThomasonAlgorithm.Core.Models;

/// <summary>
/// Represents a single step in the reconstruction of a Hamiltonian cycle during the execution of the Thomason (lollipop) algorithm.
/// </summary>
/// <param name="CurrentCycleEdges">
/// A list of edges (each as a pair of vertex indices) that form the current Hamiltonian cycle or path at this step.
/// </param>
/// <param name="AddedEdge">
/// The edge that was added to the cycle in this step, if any. Represented as a list of two vertex indices.
/// Null if no edge was added in this step.
/// </param>
/// <param name="RemovedEdge">
/// The edge that was removed from the cycle in this step, if any. Represented as a list of two vertex indices.
/// Null if no edge was removed in this step (e.g., the final step that completes the cycle).
/// </param>
public record LollipopStep(List<List<int>> CurrentCycleEdges, List<int>? AddedEdge, List<int>? RemovedEdge);