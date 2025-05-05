using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ThomasonAlgorithm.Core.Extensions;
using ThomasonAlgorithm.Core.Graphs;
using ThomasonAlgorithm.Core.Models;

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
    /// <param name="cubicGraphWithCycle">
    /// An object that encapsulates a cubic graph and one of its Hamiltonian cycles.
    /// </param>
    /// <returns>
    /// The number of steps performed by the algorithm during the reconstruction of the second cycle.
    /// </returns>
    /// <remarks>
    /// The provided Hamiltonian cycle will be modified in-place to represent the second Hamiltonian cycle.
    /// Make sure to pass a deep copy if the original cycle should be preserved.
    /// </remarks>
    public static int FindSecondHamiltonianCycleAndReturnSteps(CubicGraphWithCycle cubicGraphWithCycle)
    {
        var steps = ReconstructCycleAndReturnStepCount(cubicGraphWithCycle.HamiltonianCycle, cubicGraphWithCycle.Graph);
        
        return steps;
    }
    
    /// <summary>
    /// Finds the second Hamiltonian cycle in a cubic graph that already includes an initial Hamiltonian cycle,
    /// and collects visualization steps for the transformation using Thomason’s algorithm.
    /// </summary>
    /// <param name="cubicGraphWithCycle">
    /// A <see cref="CubicGraphWithCycle"/> instance containing both the cubic graph and its initial Hamiltonian cycle.
    /// </param>
    public static void FindSecondHamiltonianCycleAndVisualize(CubicGraphWithCycle cubicGraphWithCycle)
    {
        var steps = ReconstructCycleAndReturnSteps(cubicGraphWithCycle.HamiltonianCycle, cubicGraphWithCycle.Graph);
        RunVisualization(steps, cubicGraphWithCycle.Graph.AdjacencyMatrix);
    }
    
    /// <summary>
    /// Constructs a cubic graph from the given adjacency matrix, computes an initial Hamiltonian cycle,
    /// and finds the second Hamiltonian cycle using Thomason’s algorithm, producing visualization steps.
    /// </summary>
    /// <param name="adjacencyMatrix">
    /// A square 2D array representing the adjacency matrix of a cubic graph (each row contains exactly 3 ones).
    /// </param>
    public static void FindSecondHamiltonianCycleAndVisualize(int[,] adjacencyMatrix)
    {
        var cubicGraph = new CubicGraph(adjacencyMatrix);
        var cubicGraphWithCycle = new CubicGraphWithCycle(cubicGraph);
        var steps = ReconstructCycleAndReturnSteps(cubicGraphWithCycle.HamiltonianCycle, cubicGraphWithCycle.Graph);
        
        RunVisualization(steps, cubicGraphWithCycle.Graph.AdjacencyMatrix);
    }
    
    /// <summary>
    /// Parses a cubic graph from a compact string representation of neighbor lists,
    /// constructs an initial Hamiltonian cycle, and applies Thomason’s algorithm
    /// to find a second Hamiltonian cycle while collecting visualization steps.
    /// </summary>
    /// <param name="stringOfNeighbors">
    /// A string where each vertex’s neighbors are given as comma-separated integers,
    /// and each vertex entry is separated by an underscore '_'.<br/>
    /// For example: <c>"1,2,3_0,2,4_0,1,5_..."</c> represents a graph where vertex 0 is connected to 1, 2, 3,
    /// vertex 1 is connected to 0, 2, 4, and so on.
    /// </param>
    /// <remarks>
    /// Internally uses <see cref="GetMatrixFromNeighborsString"/> to convert the string into an adjacency matrix.
    /// </remarks>
    public static void FindSecondHamiltonianCycleAndVisualize(string stringOfNeighbors)
    {
        var adjacencyMatrix = GetMatrixFromNeighborsString(stringOfNeighbors);
        var cubicGraph = new CubicGraph(adjacencyMatrix);
        var cubicGraphWithCycle = new CubicGraphWithCycle(cubicGraph);
        var steps = ReconstructCycleAndReturnSteps(cubicGraphWithCycle.HamiltonianCycle, cubicGraphWithCycle.Graph);
        RunVisualization(steps, cubicGraphWithCycle.Graph.AdjacencyMatrix);
    }
    
    /// <summary>
    /// Generates interactive visualization files for the lollipop algorithm's execution on a cubic graph.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method creates a complete visualization package consisting of:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>A <c>VisualizeSteps</c> directory created at the root of the calling project</description></item>
    ///     <item><description><c>index.html</c> - Self-contained visualization viewer with interactive controls</description></item>
    ///     <item><description><c>steps.json</c> - Complete algorithm execution trace with graph state at each step</description></item>
    /// </list>
    /// <para>
    /// The visualization allows examining:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>The complete Hamiltonian cycle reconstruction process</description></item>
    ///     <item><description>Intermediate graph states during cycle extension</description></item>
    ///     <item><description>Edge transformations during the lollipop operation</description></item>
    /// </list>
    /// </remarks>
    /// <param name="cubicGraphWithCycle">
    /// The input cubic graph with Hamiltonian cycle to visualize. 
    /// Requires:
    /// <list type="bullet">
    ///     <item><description>Valid <see cref="CubicGraphWithCycle.HamiltonianCycle"/> containing at least 3 vertices</description></item>
    ///     <item><description>Consistent <see cref="CubicGraphWithCycle.Graph"/> adjacency matrix matching the cycle</description></item>
    /// </list>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="cubicGraphWithCycle"/> or its essential components are null
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the graph and cycle fail validation checks
    /// </exception>
    /// <example>
    /// Basic usage:
    /// <code>
    /// var graph = CubicGraphWithCycle.FromAdjacencyMatrix(matrix, cycle);
    /// LollipopVisualizer.CreateLollipopVisualization(graph);
    /// // Opens visualization at: [ProjectRoot]/VisualizeSteps/index.html
    /// </code>
    /// </example>
    public static void CreateLollipopVisualization(CubicGraphWithCycle cubicGraphWithCycle)
    {
        var steps = ReconstructCycleAndReturnSteps(cubicGraphWithCycle.HamiltonianCycle, cubicGraphWithCycle.Graph);
        CreateVisualizationInProjectRoot(steps, cubicGraphWithCycle.Graph.AdjacencyMatrix);
    }
    
    /// <summary>
    /// Generates interactive visualization of the lollipop algorithm from an adjacency matrix.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Creates a complete visualization package in the calling project's root directory under <c>VisualizeSteps/</c> containing:
    /// </para>
    /// <list type="bullet">
    ///     <item><description><c>index.html</c> - Interactive visualization viewer</description></item>
    ///     <item><description><c>steps.json</c> - Complete algorithm execution trace</description></item>
    /// </list>
    /// <para>
    /// The visualization demonstrates:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Hamiltonian cycle reconstruction process</description></item>
    ///     <item><description>Graph transformations during lollipop operations</description></item>
    ///     <item><description>Edge substitutions and cycle extensions</description></item>
    /// </list>
    /// </remarks>
    /// <param name="adjacencyMatrix">
    /// The adjacency matrix representing a cubic graph. Requirements:
    /// <list type="bullet">
    ///     <item><description>3-regular graph matrix (each vertex has degree 3)</description></item>
    ///     <item><description>Square matrix with consistent connectivity</description></item>
    /// </list>
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when adjacencyMatrix is null</exception>
    /// <exception cref="ArgumentException">Thrown when matrix doesn't represent a valid cubic graph</exception>
    /// <example>
    /// <code>
    /// int[,] matrix = new int[,] { {0,1,1,1}, {1,0,1,1}, {1,1,0,1}, {1,1,1,0} };
    /// LollipopVisualizer.CreateLollipopVisualization(matrix);
    /// </code>
    /// </example>
    public static void CreateLollipopVisualization(int[,] adjacencyMatrix)
    {
        var cubicGraph = new CubicGraph(adjacencyMatrix);
        var cubicGraphWithCycle = new CubicGraphWithCycle(cubicGraph);
        var steps = ReconstructCycleAndReturnSteps(cubicGraph.HamiltonianCycle, cubicGraphWithCycle.Graph);
        CreateVisualizationInProjectRoot(steps, cubicGraphWithCycle.Graph.AdjacencyMatrix);
    }
    
    /// <summary>
    /// Generates interactive visualization of the lollipop algorithm from a compressed neighbor list string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Creates a complete visualization package in the calling project's root directory under <c>VisualizeSteps/</c> containing:
    /// </para>
    /// <list type="bullet">
    ///     <item><description><c>index.html</c> - Interactive visualization viewer</description></item>
    ///     <item><description><c>steps.json</c> - Complete algorithm execution trace</description></item>
    /// </list>
    /// <para>
    /// <strong>Input String Format:</strong>
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Vertex neighbors are separated by underscores (<c>_</c>)</description></item>
    ///     <item><description>Each vertex entry contains exactly 3 comma-separated neighbor indices</description></item>
    ///     <item><description>Vertex numbering starts at 0</description></item>
    ///     <item><description>Example: <c>0,1,2_1,0,3_2,0,3_3,1,2</c> for a 4-vertex cubic graph</description></item>
    /// </list>
    /// <para>
    /// The visualization demonstrates:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Hamiltonian cycle reconstruction process</description></item>
    ///     <item><description>Graph transformations during lollipop operations</description></item>
    ///     <item><description>Edge substitutions and cycle extensions</description></item>
    /// </list>
    /// </remarks>
    /// <param name="stringOfNeighbors">
    /// Compressed string representation of graph connectivity with:
    /// <list type="bullet">
    ///     <item><description>Underscore-delimited vertex entries</description></item>
    ///     <item><description>Exactly 3 comma-separated neighbors per vertex</description></item>
    ///     <item><description>Zero-based vertex numbering</description></item>
    ///     <item><description>Symmetric relationships (if A lists B, B must list A)</description></item>
    /// </list>
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when input string is null</exception>
    /// <exception cref="FormatException">
    /// Thrown when:
    /// <list type="bullet">
    ///     <item><description>String doesn't match expected pattern</description></item>
    ///     <item><description>Vertex has incorrect number of neighbors</description></item>
    ///     <item><description>Contains non-numeric values</description></item>
    /// </list>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when:
    /// <list type="bullet">
    ///     <item><description>Neighbor list doesn't form a valid cubic graph</description></item>
    ///     <item><description>Graph is not 3-regular</description></item>
    ///     <item><description>Asymmetric relationships detected</description></item>
    /// </list>
    /// </exception>
    /// <example>
    /// For a cube graph:
    /// <code>
    /// string cubeNeighbors = "0,1,4_1,0,2,5_2,1,3,6_3,2,7_4,0,5,7_5,1,4,6_6,2,5,7_7,3,4,6";
    /// LollipopVisualizer.CreateLollipopVisualization(cubeNeighbors);
    /// </code>
    /// </example>
    public static void CreateLollipopVisualization(string stringOfNeighbors)
    {
        var adjacencyMatrix = GetMatrixFromNeighborsString(stringOfNeighbors);
        var cubicGraph = new CubicGraph(adjacencyMatrix);
        var cubicGraphWithCycle = new CubicGraphWithCycle(cubicGraph);
        var steps = ReconstructCycleAndReturnSteps(cubicGraph.HamiltonianCycle, cubicGraphWithCycle.Graph);
        CreateVisualizationInProjectRoot(steps, cubicGraphWithCycle.Graph.AdjacencyMatrix);
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
    /// Performs a step-by-step reconstruction of a Hamiltonian cycle using a Thomason algorithm
    /// and returns a list of steps for visualization, including added and removed edges.
    /// </summary>
    /// <param name="hamiltonianCycle">
    /// A dictionary representing the initial Hamiltonian cycle. Each key is a vertex, and its value is a list of two neighboring vertices in the cycle.
    /// Note: During the algorithm execution, some values may be temporarily set to -1 to represent removed edges.
    /// </param>
    /// <param name="graph">
    /// The cubic graph in which the Hamiltonian cycle is reconstructed. Used to determine adjacency information.
    /// </param>
    /// <returns>
    /// A list of <see cref="LollipopStep"/> representing each step of the cycle reconstruction process.
    /// Each step includes:
    /// - The current set of edges in the Hamiltonian cycle (<c>CurrentCycleEdges</c>),
    /// - The removed edge (<c>RemovedEdge</c>),
    /// - The added edge (<c>AddedEdge</c>).
    /// The final step contains only the closing edge of the cycle.
    /// </returns>
    /// <remarks>
    /// This method mutates the <paramref name="hamiltonianCycle"/> during execution.
    /// It is recommended to pass a deep copy of the original cycle to avoid modifying the original data structure.
    /// </remarks>
    private static List<LollipopStep> ReconstructCycleAndReturnSteps(Dictionary<int, List<int>> hamiltonianCycle, CubicGraph graph)
    {
        var steps = new List<LollipopStep>();
        var startVertex = 0;

        var neighborToUnlink = hamiltonianCycle[startVertex][0];
        hamiltonianCycle[startVertex][0] = -1;

        var startIndex = hamiltonianCycle[neighborToUnlink].IndexOf(startVertex);
        hamiltonianCycle[neighborToUnlink][startIndex] = -1;

        var nextVertex = GetNewEdgeEnd(startVertex, neighborToUnlink, hamiltonianCycle, graph);

        while (nextVertex != startVertex)
        {
            var nextNeighborToUnlink = GetNeighborToUnlink(nextVertex, hamiltonianCycle);

            var unlinkIndex = hamiltonianCycle[nextVertex].IndexOf(nextNeighborToUnlink);
            hamiltonianCycle[nextVertex][unlinkIndex] = neighborToUnlink;

            var idx2 = hamiltonianCycle[nextNeighborToUnlink].IndexOf(nextVertex);
            hamiltonianCycle[nextNeighborToUnlink][idx2] = -1;


            steps.Add(new LollipopStep
            (
                CurrentCycleEdges: ExtractEdgesFromCycle(hamiltonianCycle),
                RemovedEdge: [nextVertex, nextNeighborToUnlink],
                AddedEdge: [nextVertex, neighborToUnlink]
            ));

            neighborToUnlink = nextNeighborToUnlink;
            nextVertex = GetNewEdgeEnd(nextVertex, neighborToUnlink, hamiltonianCycle, graph);
        }

        var emptyIndex = hamiltonianCycle[startVertex].IndexOf(-1);
        hamiltonianCycle[startVertex][emptyIndex] = neighborToUnlink;

        steps.Add(new LollipopStep
        (
            CurrentCycleEdges: ExtractEdgesFromCycle(hamiltonianCycle),
            RemovedEdge: null,
            AddedEdge: [startVertex, neighborToUnlink]
        ));

        return steps;
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
    /// <param name="traversalStartingVertex">The starting vertex of the traversal.</param>
    /// <param name="neighbor">The neighbor vertex to move toward initially.</param>
    /// <param name="hamiltonianCycle">The current Hamiltonian cycle structure (may be partially modified).</param>
    /// <returns>
    /// True if a complete cycle is detected by traversing neighbor;
    /// False if the traversal leads to an unconnected vertex, indicating no cycle.
    /// </returns>
    private static bool CheckCycle(int traversalStartingVertex, int neighbor, Dictionary<int, List<int>> hamiltonianCycle)
    {
        var visited = new HashSet<int> { traversalStartingVertex };
        var currentVertex = neighbor;

        while (visited.Add(currentVertex))
        {
            var neighborsInCycle = GetVertexNeighborsInCycle(currentVertex, hamiltonianCycle);   
            
            // If among the neighbors in the Hamiltonian cycle there are vertices
            // that have not been visited yet and are not "empty" slots (-1),
            // then continue the traversal
            if (neighborsInCycle.Any(x => !visited.Contains(x) && x != -1))
            {
                currentVertex = neighborsInCycle.First(x => !visited.Contains(x) && x != -1);
            }
            // if during traversal we have reached 0 vertex - we are in the beginning of the hamiltonian path
            // and it means that no cycle was found
            if (currentVertex == 0)
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
    
    /// <summary>
    /// Converts a compressed neighbor string representation into an adjacency matrix.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Expected string format: "0,1,2_1,0,3_2,0,3_3,1,2" where:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Underscores (_) separate vertex entries</description></item>
    ///     <item><description>Commas separate neighbor indices within each vertex</description></item>
    ///     <item><description>Each vertex must list exactly 3 neighbors (for cubic graphs)</description></item>
    ///     <item><description>Vertex numbering starts at 0</description></item>
    /// </list>
    /// </remarks>
    /// <param name="neighborsString">The compressed neighbor string to parse</param>
    /// <returns>A square adjacency matrix where matrix[i,j] = 1 indicates an edge between vertices i and j</returns>
    /// <exception cref="ArgumentNullException">Thrown when neighborsString is null or empty</exception>
    /// <exception cref="FormatException">
    /// Thrown when:
    /// <list type="bullet">
    ///     <item><description>String contains invalid numeric formats</description></item>
    ///     <item><description>Vertex entry has incorrect number of neighbors (not exactly 3)</description></item>
    /// </list>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when:
    /// <list type="bullet">
    ///     <item><description>Vertex index is out of bounds (negative or ≥ vertex count)</description></item>
    ///     <item><description>Graph contains self-loops (vertex listing itself as neighbor)</description></item>
    ///     <item><description>Graph is not symmetric (if i lists j, j must list i)</description></item>
    /// </list>
    /// </exception>
    private static int[,] GetMatrixFromNeighborsString(string neighborsString)
    {
        if (string.IsNullOrWhiteSpace(neighborsString))
            throw new ArgumentNullException(nameof(neighborsString), "Neighbor string cannot be null or empty");
        
        if (!neighborsString.Contains('_'))
            throw new FormatException(
                "Invalid neighbor string format: must contain underscore '_' delimiters between vertex entries. " +
                "Expected format: '0,1,2_1,0,3_2,0,3_3,1,2' where underscores separate vertex neighbor lists.");
        
        var neighbors = neighborsString.Split('_');
        var matrix = new int[neighbors.Length, neighbors.Length];

        for (var i = 0; i < neighbors.Length; i++)
        {
            foreach (var neib in neighbors[i].Split(','))
            {
                if (i == int.Parse(neib))
                    throw new ArgumentException($"Vertex {i} cannot be its own neighbor (self-loops not allowed)");
                
                matrix[i, int.Parse(neib)] = 1;
            }
        }

        for (var i = 0; i < neighbors.Length; i++)
        {
            for (var j = 0; j < neighbors.Length; j++)
            {
                if (matrix[i, j] != matrix[j, i])
                    throw new ArgumentException(
                        $"Asymmetric adjacency detected between vertices {i} and {j}. " +
                        "If vertex A lists vertex B as neighbor, vertex B must also list vertex A.");
            }
        }

        return matrix;
    }

    /// <summary>
    /// Generates and displays visualization of the lollipop algorithm steps.
    /// </summary>
    /// <remarks>
    /// - Creates visualization files (HTML + JSON) in VisualizeSteps directory
    /// - Launches local server to display the visualization
    /// - Uses embedded HTML template and serialized algorithm data
    /// </remarks>
    /// <param name="steps">Algorithm execution steps to visualize</param>
    /// <param name="adjacencyMatrix">Graph adjacency matrix for visualization</param>
    /// <exception cref="FileNotFoundException">If embedded HTML template is missing</exception>
    private static void RunVisualization(List<LollipopStep> steps, int[,] adjacencyMatrix)
    {
        var allEdges = GetAllEdges(adjacencyMatrix)
            .Select(edge => new List<int> { edge.u, edge.v })
            .ToList();
        
        var visData = new 
        {
            AllEdges = allEdges,
            Steps = steps
        };
        
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "ThomasonAlgorithm.Core.VisualizeSteps.index.html";
        
        var visualizeDir = Path.Combine(AppContext.BaseDirectory, "VisualizeSteps");
        Directory.CreateDirectory(visualizeDir);
        
        var destIndexPath = Path.Combine(visualizeDir, "index.html");
        using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
        {
            if (resourceStream != null)
            {
                using (var fileStream = File.Create(destIndexPath))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
            else
            {
                throw new FileNotFoundException($"Resource {resourceName} not found");
            }
        }
        
        var jsonPath = Path.Combine(visualizeDir, "steps.json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(visData, new JsonSerializerOptions
        {
            WriteIndented = true,
        }));
        
        RunServer(visualizeDir);
    }
    
    private static List<(int u, int v)> GetAllEdges(int[,] adjacencyMatrix)
    {
        var edges = new List<(int, int)>();
        var n = adjacencyMatrix.GetLength(0);

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (adjacencyMatrix[i, j] == 1)
                {
                    edges.Add((i, j));
                }
            }
        }

        return edges;
    }

    /// <summary>
    /// Starts a local HTTP server to serve visualization files and opens the index.html in browser.
    /// </summary>
    /// <remarks>
    /// - Hosts files from specified directory at http://localhost:11111/
    /// - Automatically opens index.html in default browser
    /// - Runs until Enter is pressed
    /// - Requires admin rights for port registration (first run)
    /// </remarks>
    /// <param name="visualizeDir">Directory containing visualization files</param>
    /// <exception cref="HttpListenerException">When lacking permissions for port binding</exception>
    private static void RunServer(string visualizeDir)
    {
        var listener = new HttpListener();
        var url = "http://localhost:11111/";
        listener.Prefixes.Add(url);

        try
        {
            listener.Start();
            Console.WriteLine($"Server is running: {url}");

            _ = Task.Run(async () =>
            {
                while (listener.IsListening)
                {
                    try
                    {
                        var context = await listener.GetContextAsync();
                        var request = context.Request;
                        var response = context.Response;

                        try
                        {
                            var filePath = Path.Combine(visualizeDir, 
                                string.IsNullOrEmpty(request.Url!.AbsolutePath.TrimStart('/')) 
                                    ? "index.html" 
                                    : request.Url.AbsolutePath.TrimStart('/'));

                            if (File.Exists(filePath))
                            {
                                var contentType = filePath.EndsWith(".html") ? "text/html" :
                                                 filePath.EndsWith(".json") ? "application/json" :
                                                 "application/octet-stream";
                                
                                var bytes = await File.ReadAllBytesAsync(filePath);
                                response.ContentType = $"{contentType}; charset=utf-8";
                                await response.OutputStream.WriteAsync(bytes);
                            }
                            else
                            {
                                response.StatusCode = 404;
                                var notFound = "404 Not Found"u8.ToArray();
                                await response.OutputStream.WriteAsync(notFound);
                            }
                        }
                        finally
                        {
                            response.Close();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            Process.Start(new ProcessStartInfo
            {
                FileName = $"{url}index.html",
                UseShellExecute = true
            });

            Console.WriteLine("Press Enter to stop server...");
            Console.ReadLine();
        }
        catch (HttpListenerException ex) when (ex.ErrorCode == 5)
        {
            Console.WriteLine("ERROR: Insufficient permissions. Run the command:");
            Console.WriteLine($"netsh http add urlacl url={url} ");
        }
        finally
        {
            listener.Stop();
        }
    }
    
    /// <summary>
    /// Creates visualization files (HTML and JSON) in the calling project's root directory.
    /// </summary>
    /// <remarks>
    /// Generates:
    /// 1. index.html (from embedded resources)
    /// 2. steps.json (serialized algorithm steps and graph data)
    /// in a "VisualizationSteps" subdirectory.
    /// </remarks>
    /// <param name="steps">Algorithm execution steps to visualize</param>
    /// <param name="adjacencyMatrix">Graph adjacency matrix for visualization</param>
    /// <exception cref="FileNotFoundException">When embedded HTML resource is missing</exception>
    private static void CreateVisualizationInProjectRoot(List<LollipopStep> steps, int[,] adjacencyMatrix)
    {
        var projectRoot = GetCallingProjectRootDirectory();
        
        var visualizeDir = Path.Combine(projectRoot, "VisualizationSteps");
        Directory.CreateDirectory(visualizeDir);
        
        var allEdges = GetAllEdges(adjacencyMatrix)
            .Select(edge => new List<int> { edge.u, edge.v })
            .ToList();
        
        var visData = new 
        {
            AllEdges = allEdges,
            Steps = steps
        };
        
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "ThomasonAlgorithm.Core.VisualizeSteps.index.html";
        
        var destIndexPath = Path.Combine(visualizeDir, "index.html");
        using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
        {
            if (resourceStream != null)
            {
                using (var fileStream = File.Create(destIndexPath))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }
            else
            {
                throw new FileNotFoundException($"Resource {resourceName} not found");
            }
        }
        
        var jsonPath = Path.Combine(visualizeDir, "steps.json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(visData, new JsonSerializerOptions
        {
            WriteIndented = true,
        }));
        
        Console.WriteLine($"Files are created in: {visualizeDir}");
    }

    private static string GetCallingProjectRootDirectory()
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        var callingAssemblyLocation = callingAssembly.Location;
        
        var directory = new DirectoryInfo(Path.GetDirectoryName(callingAssemblyLocation));
        
        while (directory != null)
        {
            if (directory.GetFiles("*.csproj").Length > 0 || 
                directory.GetFiles("*.sln").Length > 0)
            {
                return directory.FullName;
            }
            
            directory = directory.Parent;
        }
        
        return Directory.GetCurrentDirectory();
    }
        
    private static List<List<int>> ExtractEdgesFromCycle(Dictionary<int, List<int>> cycle)
    {
        var edges = new HashSet<List<int>>();
        foreach (var kvp in cycle)
        {
            int u = kvp.Key;
            foreach (int v in kvp.Value)
            {
                if (v != -1)
                {
                    var edge = u < v ? new List<int>{u, v} : new List<int>{v, u};
                    edges.Add(edge);
                }
            }
        }
        return edges.ToList();
    }
}