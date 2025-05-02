using ThomasonAlgorithm.Core.Extensions;
using ThomasonAlgorithm.Core.Graphs;

namespace ThomasonAlgorithm.Core.Algorithm;

/// <summary>
/// Provides a static implementation of Thomason's Lollipop Algorithm for transforming
/// an existing Hamiltonian cycle in a cubic graph into a second distinct Hamiltonian cycle.
/// </summary>
/// <remarks>
/// The algorithm requires a valid Hamiltonian cycle to be provided as input. It modifies a copy of the
/// input cycle without altering the original, allowing non-destructive exploration of Hamiltonian paths.
/// <para>
/// It is assumed that the input Hamiltonian cycle starts at vertex 0.
/// The first edge removed during the algorithm's execution is (0, n-1).
/// </para>
/// </remarks>
public static class LollipopAlgorithm
{
    /// <summary>
    /// Applies the Thomason's lollipop algorithm to reconstruct a second Hamiltonian cycle
    /// from the one provided in the <see cref="CubicGraphWithCycle"/> instance,
    /// and returns the number of steps taken by the algorithm.
    /// </summary>
    /// <param name="cubicGraph">
    /// An object that encapsulates a cubic graph and one of its Hamiltonian cycles.
    /// </param>
    /// <returns>
    /// The number of steps performed by the algorithm during the reconstruction of the second cycle.
    /// </returns>
    /// <remarks>
    /// The provided Hamiltonian cycle will be modified in-place to represent the second Hamiltonian cycle.
    /// Make sure to pass a deep copy if the original cycle should be preserved.
    /// </remarks>
    public static int FindSecondHamiltonianCycleAndReturnSteps(CubicGraphWithCycle cubicGraph)
    {
        var steps = ReconstructCycleAndReturnStepCount(cubicGraph.HamiltonianCycle, cubicGraph.Graph);
        
        return steps;
    }
    
    /// <summary>
    /// Applies Thomason's lollipop algorithm to reconstruct a second Hamiltonian cycle
    /// from the provided Hamiltonian cycle and returns the number of algorithm steps performed.
    /// </summary>
    /// <param name="hamiltonianCycle">
    /// A dictionary representing the original Hamiltonian cycle. The keys are vertex indices,
    /// and the values are lists of neighboring vertices in the cycle.
    /// This structure will be modified in-place to represent the new cycle.
    /// </param>
    /// <param name="graph">
    /// The cubic graph on which the algorithm is executed. It provides adjacency information
    /// necessary to guide the reconstruction process.
    /// </param>
    /// <returns>
    /// The total number of steps taken by the algorithm during the reconstruction of the cycle.
    /// </returns>
    /// <remarks>
    /// The method mutates the provided <paramref name="hamiltonianCycle"/> by breaking and reconnecting edges
    /// in accordance with Thomason's lollipop algorithm until a second Hamiltonian cycle is formed.
    /// The cycle is expected to be valid and consistent with the graph structure at the start of execution.
    /// </remarks>
    private static int ReconstructCycleAndReturnStepCount(Dictionary<int, List<int>> hamiltonianCycle, CubicGraph graph)
    {
        var startVertex = 0;
        var algorithmSteps = 0;
        
        // Disconnect the start vertex from its first neighbor
        var neighborToUnlink = hamiltonianCycle[startVertex][0];
        hamiltonianCycle[startVertex][0] = -1;
        
        // Removing start vertex from in-cycle neighbors of neighborToUnlink
        var startVertexIndex = hamiltonianCycle[neighborToUnlink].IndexOf(startVertex);
        hamiltonianCycle[neighborToUnlink][startVertexIndex] = -1;

        // Among all neighbors of the disconnected vertex, find the one
        // whose incident edge is not currently part of the cycle and is not the edge just removed
        var nextVertex = GetNewEdgeEnd(startVertex, neighborToUnlink, hamiltonianCycle, graph);

        // One algorithm step:
        // remove the edge (u, v) =>
        // connect v to a neighbor not in the cycle and not equal to the recently removed one (call it w) =>
        // after this, w will have all 3 edges included in the cycle =>
        // remove the edge (w, d), choosing it to break the newly formed cycle =>
        // the algorithm proceeds at vertex d with a new Hamiltonian path
        
        while (nextVertex != startVertex)
        {
            // nextVertex is a vertex of degree 3. Find the edge (neighbor) that should be excluded from the current Hamiltonian cycle
            var nextNeighborToUnlink = GetNeighborToUnlink(nextVertex, hamiltonianCycle);
            
            // In the set of cycle neighbors of nextVertex,
            // replace the vertex disconnected from nextVertex with the one from which the last edge was added
            var unlinkVertexIndex = hamiltonianCycle[nextVertex].IndexOf(nextNeighborToUnlink);
            hamiltonianCycle[nextVertex][unlinkVertexIndex] = neighborToUnlink;
            
            // Remove nextVertex from the in-cycle neighbor list of the disconnected vertex
            var neighborToUnlinkIndex = hamiltonianCycle[nextNeighborToUnlink].IndexOf(nextVertex);
            hamiltonianCycle[nextNeighborToUnlink][neighborToUnlinkIndex] = -1;
            // algorithm step is over
            ++algorithmSteps;
            
            nextVertex = GetNewEdgeEnd(nextVertex, nextNeighborToUnlink, hamiltonianCycle, graph);
            neighborToUnlink = nextNeighborToUnlink;
        }
        var startIndexEmptyNeighbor = hamiltonianCycle[startVertex].IndexOf(-1);
        hamiltonianCycle[startVertex][startIndexEmptyNeighbor] = neighborToUnlink;
        ++algorithmSteps;
        
        return algorithmSteps;
    }
    
    /// <summary>
    /// Returns a list of neighbors for the specified vertex in the given cubic graph.
    /// </summary>
    /// <param name="v">The vertex for which neighbors are to be found.</param>
    /// <param name="graph">The cubic graph containing the adjacency matrix.</param>
    /// <returns>A list of vertex indices that are neighbors of the specified vertex.</returns>
    private static List<int> GetVertexNeighbors(int v, CubicGraph graph)
    {
        var neighbors = new List<int>();

        for (var i = 0; i < graph.AdjacencyMatrix.GetLength(0); ++i)
        {
            if (graph.AdjacencyMatrix[v, i] == 1)
                neighbors.Add(i);
        }
        
        return neighbors;
    }

    /// <summary>
    /// Returns the list of neighbors of the specified vertex within the Hamiltonian cycle.
    /// </summary>
    /// <param name="v">The vertex whose neighbors in the cycle are to be retrieved.</param>
    /// <param name="hamiltonianCycle">The Hamiltonian cycle represented as a dictionary of vertex adjacency lists.</param>
    /// <returns>A list of neighbors of the specified vertex within the Hamiltonian cycle.</returns>
    private static List<int> GetVertexNeighborsInCycle(int v, Dictionary<int, List<int>> hamiltonianCycle)
    {
        return hamiltonianCycle[v];
    }

    /// <summary>
    /// Performs one of the steps of the lollipop algorithm by extending the current Hamiltonian path.
    /// For a vertex of degree 1 in the path (i.e., connected by only one edge),
    /// this method finds a neighbor not yet included in the cycle (and not the previously unlinked vertex),
    /// and adds an edge to that neighbor in the cycle.
    /// </summary>
    /// <param name="previousNeib">The vertex that was just disconnected from the current vertex.</param>
    /// <param name="currentVertex">The current vertex of degree 1 in the cycle that needs to be extended.</param>
    /// <param name="hamiltonianCycle">The current state of the Hamiltonian cycle, where some vertices may have -1 among neighbors.</param>
    /// <param name="graph">The full cubic graph structure, used to get all neighbors of the current vertex.</param>
    /// <returns>The neighbor vertex that was added to the Hamiltonian path.</returns>
    private static int GetNewEdgeEnd(int previousNeib, int currentVertex, Dictionary<int, List<int>> hamiltonianCycle, CubicGraph graph)
    {
        var currentVertexNeighbors = GetVertexNeighbors(currentVertex, graph);
        var currentVertexNeighborsInCycle = GetVertexNeighborsInCycle(currentVertex, hamiltonianCycle);
        var emptyIndex = currentVertexNeighborsInCycle.IndexOf(-1);
        
        var neighborToAdd = currentVertexNeighbors.First(x => !currentVertexNeighborsInCycle.Contains(x) && x != previousNeib);
        currentVertexNeighborsInCycle[emptyIndex] = neighborToAdd;

        return neighborToAdd;
    }
    
    /// <summary>
    /// Determines which neighbor of the current vertex should be unlinked from the Hamiltonian path.
    /// This is used when the current vertex has degree 3 in the path, and the algorithm needs to break
    /// an internal cycle formed within the Hamiltonian path.
    /// The method checks both directions of the path from the current vertex to identify the correct neighbor
    /// to disconnect in order to continue the reconstruction.
    /// </summary>
    /// <param name="currentVertex">The vertex of degree 3 in the Hamiltonian path from which the cycle should be broken.</param>
    /// <param name="hamiltonianCycle">The current state of the Hamiltonian cycle.</param>
    /// <returns>The neighbor vertex to be unlinked from the cycle.</returns>
    private static int GetNeighborToUnlink(int currentVertex, Dictionary<int, List<int>> hamiltonianCycle)
    {
        var currentVertexNeighborsInCycle = GetVertexNeighborsInCycle(currentVertex, hamiltonianCycle);

        if (CheckCycle(currentVertex, currentVertexNeighborsInCycle.First(), hamiltonianCycle))
            return currentVertexNeighborsInCycle.First();
        
        return currentVertexNeighborsInCycle.Last();
    }

    /// <summary>
    /// Determines whether a cycle exists in the Hamiltonian path, starting from the given vertex
    /// and moving in the direction of the specified neighbor.
    /// This method is used to detect whether the traversal along the Hamiltonian cycle
    /// forms a closed loop (cycle) or reaches a vertex that is not fully connected (i.e., has a -1 entry).
    /// </summary>
    /// <param name="currentVertex">The starting vertex of the traversal.</param>
    /// <param name="neighbor">The neighbor vertex to move toward initially.</param>
    /// <param name="hamiltonianCycle">The current Hamiltonian cycle structure (may be partially modified).</param>
    /// <returns>
    /// True if a complete cycle is detected by traversing neighbor;
    /// False if the traversal leads to an unconnected vertex, indicating no cycle.
    /// </returns>
    private static bool CheckCycle(int currentVertex, int neighbor, Dictionary<int, List<int>> hamiltonianCycle)
    {
        var visited = new HashSet<int> { currentVertex };
        var observingVertex = neighbor;

        while (!visited.Contains(observingVertex))
        {
            visited.Add(observingVertex);
            
            var neighborsInCycle = GetVertexNeighborsInCycle(observingVertex, hamiltonianCycle);   
            
            // If among the neighbors in the Hamiltonian cycle there are vertices
            // that have not been visited yet and are not "empty" slots (-1),
            // then continue the traversal
            if (neighborsInCycle.Any(x => !visited.Contains(x) && x != -1))
            {
                observingVertex = neighborsInCycle.First(x => !visited.Contains(x) && x != -1);
            }
            // если среди соседей по циклу есть непосещенные вершины и среди них есть "пустые", значит, мы пришли в вершину степени 1 - цикла нет
            // TODO попробовать изменить условие
            else if (neighborsInCycle.Any(x => !visited.Contains(x)) && neighborsInCycle.Contains(-1))
            {
                return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// Gets the set of vertices in the Hamiltonian cycle, ordered in the sequence 
    /// they appear in the cycle, starting from vertex 0.
    /// </summary>
    /// <param name="hamiltonianCycle">A dictionary representing the Hamiltonian cycle,
    /// where each vertex is mapped to its list of neighbors in the cycle.</param>
    /// <returns>A <see cref="HashSet{int32}"/> containing the vertices of the Hamiltonian cycle
    /// in the order they appear in the cycle starting from 0.</returns>
    public static HashSet<int> GetHamiltonianCycleSequencedVertices(Dictionary<int, List<int>> hamiltonianCycle)
    {
        var sequence = new HashSet<int>();
        var currentVertex = 0;

        while (!sequence.Contains(currentVertex))
        {
            sequence.Add(currentVertex);
            
            var currentVertexNeighborsInCycle = GetVertexNeighborsInCycle(currentVertex, hamiltonianCycle);
            var nextVertex = currentVertexNeighborsInCycle.FirstPossibleNeighborOrNull(x => !sequence.Contains(x));
 
            currentVertex = nextVertex ?? currentVertex;
        }
        
        return sequence;        
    }
}