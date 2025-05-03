using ThomasonAlgorithm.Core.Graphs;
using ThomasonAlgorithm.Core.Models;

namespace ThomasonAlgorithm.Core.GraphGenerators;

/// <summary>
/// A utility class for generating random cubic (3-regular) graphs with a specified number of vertices.
/// The generated graph ensures that each vertex has exactly 3 neighbors, adhering to the properties of a cubic graph.
/// Additionally, this class provides methods for generating cubic graphs with various constraints on chord lengths.
/// </summary>
public static class RandomCubicGraphGenerator
{
    private static readonly Random Rand = new Random();
    
    /// <summary>
    /// Generates a cubic graph (3-regular graph) where chord lengths are restricted to be within the inclusive range [kLow, kUp].
    /// </summary>
    /// <param name="n">The number of vertices in the graph. Must be an even number ≥ 4.</param>
    /// <param name="kLow">The minimum allowed chord length (inclusive).</param>
    /// <param name="kUp">The maximum allowed chord length (inclusive).</param>
    /// <param name="maxAttempts">Maximum number of attempts to generate a valid graph. Default is 100.</param>
    /// <returns>
    /// A cubic graph where all chords have lengths within the range [<paramref name="kLow"/>, <paramref name="kUp"/>],
    /// or <c>null</c> if generation fails within the given number of attempts.
    /// </returns>
    public static CubicGraph? GenerateGraphLimitedChords(int n, int kLow, int kUp, int maxAttempts = 100)
        => GenerateCubicGraphWithChordConstraint(n, kLow, kUp, dist => dist >= kLow && dist <= kUp, maxAttempts);
    
    /// <summary>
    /// Generates a cubic graph (3-regular graph) where only chords of odd lengths within the given bounds are allowed.
    /// The resulting graph is guaranteed to be bipartite due to the restriction on odd-length chords.
    /// </summary>
    /// <param name="n">The number of vertices in the graph. Must be an even number ≥ 4.</param>
    /// <param name="kLow">The minimum chord length (inclusive).</param>
    /// <param name="kUp">The maximum chord length (inclusive).</param>
    /// <param name="maxAttempts">Maximum number of attempts to generate a valid graph. Default is 100.</param>
    /// <returns>
    /// A bipartite cubic graph containing only odd-length chords in the range [<paramref name="kLow"/>, <paramref name="kUp"/>],
    /// or <c>null</c> if generation fails within the given number of attempts.
    /// </returns>
    public static CubicGraph? GenerateGraphWithOddLengthChords(int n, int kLow, int kUp, int maxAttempts = 100)
    {
        var oddLengths = GenerateOddNumbersSet(kLow, kUp);
        return GenerateCubicGraphWithChordConstraint(n, kLow, kUp, dist => oddLengths.Contains(dist), maxAttempts);
    }
    
    /// <summary>
    /// Generates a cubic graph (3-regular graph) where chord lengths are within the specified range
    /// <paramref name="kLow"/> to <paramref name="kUp"/>, but exclude a specific gap range.
    /// </summary>
    /// <param name="n">The number of vertices in the graph. Must be an even number ≥ 4.</param>
    /// <param name="kLow">The minimum allowed chord length.</param>
    /// <param name="kUp">The maximum allowed chord length.</param>
    /// <param name="gap">
    /// A <see cref="Gap"/> struct specifying the range of chord lengths to exclude from the graph.
    /// For example, if the gap is [5–7], then chords of lengths 5, 6, and 7 will be excluded.
    /// </param>
    /// <param name="maxAttempts">Maximum number of attempts to generate a valid graph. Default is 100.</param>
    /// <returns>
    /// A cubic graph that avoids the specified gap in chord lengths, or <c>null</c> if no valid graph is found
    /// within the allowed number of attempts.
    /// </returns>
    public static CubicGraph? GenerateGraphWithGapInChordLengths(int n, int kLow, int kUp, Gap gap, int maxAttempts = 100)
    {
        var withGap = GenerateNumbersSetWithGap(kLow, kUp, gap);
        return GenerateCubicGraphWithChordConstraint(n, kLow, kUp, dist => withGap.Contains(dist), maxAttempts);
    }
    
    /// <summary>
    /// Generates a cubic graph (3-regular graph) with a precise number of chords for each specified chord length.
    /// </summary>
    /// <param name="n">The number of vertices in the graph. Must be an even number ≥ 4.</param>
    /// <param name="kLow">The minimum allowed chord length.</param>
    /// <param name="kUp">The maximum allowed chord length.</param>
    /// <param name="chordsLengthDictionary">
    /// A dictionary mapping chord lengths to the exact number of chords of that length that must appear in the graph.
    /// </param>
    /// <param name="maxAttempts">The maximum number of attempts to generate a valid graph. Default is 100.</param>
    /// <returns>
    /// A cubic graph that matches the exact chord length distribution if one is found within the allowed attempts;
    /// otherwise, returns <c>null</c>.
    /// </returns>
    public static CubicGraph? GenerateGraphFromExactChordsSet(int n, int kLow, int kUp, Dictionary<int, int> chordsLengthDictionary, int maxAttempts = 100)
        => GenerateCubicGraphFromExactChordsSet(n, kLow, kUp, chordsLengthDictionary, maxAttempts);

    /// <summary>
    /// Generates a random cubic graph (3-regular graph) where possible chord connections 
    /// are filtered using the provided predicate on chord lengths.
    /// </summary>
    /// <param name="n">The number of vertices in the graph. Must be an even number ≥ 4.</param>
    /// <param name="kLow">The minimum allowed chord length.</param>
    /// <param name="kUp">The maximum allowed chord length.</param>
    /// <param name="chordLengthPredicate">A predicate function that determines whether a given chord length is allowed.</param>
    /// <param name="maxAttempts">The maximum number of attempts to generate a valid graph.</param>
    /// <returns>
    /// A cubic graph satisfying the chord constraint if generation succeeds within the attempt limit;
    /// otherwise, returns <c>null</c>.
    /// </returns>
    private static CubicGraph? GenerateCubicGraphWithChordConstraint(
        int n, 
        int kLow, 
        int kUp,
        Func<int, bool> chordLengthPredicate, 
        int maxAttempts = 100)
    {
        ValidateInput(n, kLow, kUp);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var graph = CreateInitialCycleGraph(n);
            var possibleNeighbors = GetPossibleNeighbors(n, chordLengthPredicate);

            var matching = FindPerfectMatching(n, possibleNeighbors, graph);
            if (matching != null)
            {
                ApplyMatchingToGraph(graph, matching);
                if (graph.IsCubic())
                    return graph;
            }
        }

        return null;
    }

    /// <summary>
    /// Generates a cubic graph (3-regular graph) where the set of chords exactly matches the specified count for each chord length.
    /// </summary>
    /// <param name="n">The number of vertices in the graph. Must be an even number ≥ 4.</param>
    /// <param name="kLow">The minimum possible chord length.</param>
    /// <param name="kUp">The maximum possible chord length.</param>
    /// <param name="chordsLengthDictionary">
    /// A dictionary specifying the exact number of chords required for each chord length.
    /// Keys are chord lengths, values are the number of chords of that length required in the graph.
    /// </param>
    /// <param name="maxAttempts">The maximum number of attempts to generate a valid graph.</param>
    /// <returns>
    /// A cubic graph satisfying the exact chord length distribution if found within the allowed attempts;
    /// otherwise, returns <c>null</c>.
    /// </returns>
    private static CubicGraph? GenerateCubicGraphFromExactChordsSet(
        int n, 
        int kLow, 
        int kUp,
        Dictionary<int, int> chordsLengthDictionary, 
        int maxAttempts = 100)
    {
        ValidateInput(n, kLow, kUp);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var graph = CreateInitialCycleGraph(n);
            var neededChords = new int[kUp + 1];
        
            foreach (var kvp in chordsLengthDictionary)
            {
                var length = kvp.Key;
                var number = kvp.Value;

                neededChords[length] = number;
            }
            var possibleNeighbors = GetPossibleNeighbors(n, dist => dist >= kLow && dist <= kUp);

            var matching = FindPerfectMatching(n, possibleNeighbors, graph, neededChords);
            if (matching != null)
            {
                ApplyMatchingToGraph(graph, matching);
                if (graph.IsCubic())
                    return graph;
            }
        }
        return null;
    }


    private static void ValidateInput(int n, int kLow, int kUp)
    {
        if (n < 4 || n % 2 != 0)
            throw new ArgumentException("Cubic graph can be constructed with n >= 4 only.");
        if (kLow <= 1 || kUp > n / 2)
            throw new ArgumentException("Wrong K value");
    }

    private static CubicGraph CreateInitialCycleGraph(int n)
    {
        var graph = new CubicGraph(n);
        for (var i = 0; i < n; i++)
        {
            var j = (i + 1) % n;
            graph.AddEdge(i, j);
        }
        return graph;
    }

    /// <summary>
    /// Generates a dictionary of possible neighbors for each vertex in a graph,
    /// based on the provided chord length predicate. This predicate is used to 
    /// filter the valid chord lengths between vertices in the graph.
    /// </summary>
    /// <param name="n">The number of vertices in the graph.</param>
    /// <param name="chordLengthPredicate">A predicate function that determines 
    /// whether a chord length between two vertices is valid.</param>
    /// <returns>A dictionary where the key is a vertex, and the value is a 
    /// HashSet of neighboring vertices that satisfy the chord length condition.</returns>
    private static Dictionary<int, HashSet<int>> GetPossibleNeighbors(int n, Func<int, bool> chordLengthPredicate)
    {
        var possibleNeighbors = new Dictionary<int, HashSet<int>>();
        
        for (int i = 0; i < n; i++)
        {
            possibleNeighbors.Add(i, new HashSet<int>());
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 2; j < n; j++)
            {
                if (j == (i - 1 + n) % n)
                    continue; // сосед по циклу (конец сегмента)

                var dist = GetChordLength(i, j, n);
                
                if (chordLengthPredicate(dist))
                {
                    possibleNeighbors[i].Add(j);
                    possibleNeighbors[j].Add(i);
                }
            }
        }
        return possibleNeighbors;
    }
    
    private static int GetChordLength(int i, int j, int n) => Math.Min(Math.Abs(i - j), n - Math.Abs(i - j));

    private static void ApplyMatchingToGraph(CubicGraph graph, List<(int, int)> matching)
    {
        foreach (var (u, v) in matching)
        {
            graph.AddEdge(u, v);
        }
    }
    
    /// <summary>
    /// Tries to find a perfect matching in the given graph using the provided possible neighbors for each vertex.
    /// A perfect matching is a set of edges such that every vertex is matched with exactly one other vertex.
    /// </summary>
    /// <param name="n">The number of vertices in the graph.</param>
    /// <param name="possibleNeighbors">A dictionary where the key is a vertex, and the value is a HashSet of neighboring vertices 
    /// that could potentially be matched with the vertex.</param>
    /// <param name="graph">The graph in which the perfect matching is being searched for. The graph will be updated with the 
    /// lengths of the chords that form part of the matching.</param>
    /// <returns>A list of pairs of vertices representing the perfect matching, or null if a perfect matching is not found.</returns>
    /// <remarks>
    /// The method tries to find a random matching for each vertex, ensuring that no vertex is used in more than one edge 
    /// and that the number of edges in the matching matches the total number of vertices in the graph.
    /// If a perfect matching cannot be found for all vertices, the method will return null.
    /// </remarks>
    private static List<(int, int)>? FindPerfectMatching(int n, Dictionary<int, HashSet<int>> possibleNeighbors, CubicGraph graph)
    {
        var needed = GetNumberNeededEdges(n, graph);
        var totalNeeded = needed.Sum();

        if (totalNeeded != n)
            return null;

        var usedInMatching = new bool[n];
        var result = new List<(int,int)>();
        
        for (var i = 0; i < n; i++)
        {
            var possibleNeighborsList = possibleNeighbors[i].ToList();
            var checkedNeighbors = new HashSet<int>();

            if (possibleNeighborsList.Count == 0)
                return null;

            Shuffle(possibleNeighborsList);

            while (!usedInMatching[i] && checkedNeighbors.Count < possibleNeighborsList.Count)
            {
                var randomIndex = Rand.Next(possibleNeighborsList.Count);
                var selectedNeighbor = possibleNeighborsList[randomIndex];
                checkedNeighbors.Add(selectedNeighbor);

                if (!usedInMatching[selectedNeighbor])
                {
                    result.Add((i, selectedNeighbor));
                    usedInMatching[i] = true;
                    usedInMatching[selectedNeighbor] = true;

                    var chordLength = GetChordLength(i, selectedNeighbor, n);
                    graph.AddChordLength(chordLength);
                }
            }

            if (!usedInMatching[i])
            {
                return null;
            }
        }
        return result;
    }

    /// <summary>
    /// Tries to find a perfect matching in the graph, where the number of edges with specific chord lengths 
    /// corresponds exactly to the provided `neededChords` array. Each entry in `neededChords` specifies 
    /// the required number of edges of a particular length.
    /// </summary>
    /// <param name="n">The number of vertices in the graph.</param>
    /// <param name="possibleNeighbors">A dictionary where the key is a vertex, and the value is a HashSet of neighboring vertices 
    /// that could potentially be matched with the vertex.</param>
    /// <param name="graph">The graph in which the perfect matching is being searched for. The graph will be updated with the 
    /// lengths of the chords that form part of the matching.</param>
    /// <param name="neededChords">An array where the index represents the chord length, and the value at each index 
    /// specifies how many edges of that particular length are needed in the perfect matching.</param>
    /// <returns>A list of pairs of vertices representing the perfect matching, or null if a perfect matching is not found.</returns>
    /// <remarks>
    /// The method tries to find a random matching for each vertex while ensuring that the number of edges of each 
    /// chord length in the matching matches the number of times that length is needed as specified in the `neededChords` array.
    /// If a perfect matching cannot be found that satisfies the required chord lengths, the method will return null.
    /// </remarks>
    private static List<(int, int)>? FindPerfectMatching(
        int n,
        Dictionary<int, HashSet<int>> possibleNeighbors,
        CubicGraph graph,
        int[] neededChords)
    {
        var needed = GetNumberNeededEdges(n, graph);
        var totalNeeded = needed.Sum();

        if (totalNeeded != n)
            return null;

        var usedInMatching = new bool[n];
        var result = new List<(int,int)>();
        
        for (var i = 0; i < n; i++)
        {
            var possibleNeighborsList = possibleNeighbors[i].ToList();
            var checkedNeighbors = new HashSet<int>();

            if (possibleNeighborsList.Count == 0)
                return null;

            Shuffle(possibleNeighborsList);

            while (!usedInMatching[i] && checkedNeighbors.Count < possibleNeighborsList.Count)
            {
                var randomIndex = Rand.Next(possibleNeighborsList.Count);
                var selectedNeighbor = possibleNeighborsList[randomIndex];
                checkedNeighbors.Add(selectedNeighbor);
                var chordLength = GetChordLength(i, selectedNeighbor, n); // ВНИМАТЕЛЬНО ПРОТЕСТИРУЙ ЭТО

                if (!usedInMatching[selectedNeighbor] && neededChords[chordLength] != 0)
                {
                    result.Add((i, selectedNeighbor));
                    usedInMatching[i] = true;
                    usedInMatching[selectedNeighbor] = true;
                    neededChords[chordLength]--;

                    graph.AddChordLength(chordLength);
                }
            }

            if (!usedInMatching[i])
            {
                return null;
            }
        }
        return result;
    }

    private static int[] GetNumberNeededEdges(int n, CubicGraph graph)
    {
        var needed = new int[n];
        for (var i = 0; i < n; i++)
        {
            var deg = graph.GetVertexDegree(i);
            if (deg > 3) 
                throw new InvalidOperationException($"Vertex {i} has degree {deg}, which exceeds 3. Graph is not cubic.");
            needed[i] = 3 - deg;
        }
        return needed;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = Rand.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    
    private static HashSet<int> GenerateOddNumbersSet(int leftBound, int rightBound)
    {
        return Enumerable.Range(leftBound, rightBound - leftBound + 1)
            .Where(i => i % 2 != 0)
            .ToHashSet();
    }
    
    private static HashSet<int> GenerateNumbersSetWithGap(int leftBound, int rightBound, Gap gap)
    {
        return Enumerable.Range(leftBound, rightBound - leftBound + 1)
            .Where(i => i < gap.LeftBound || i > gap.RightBound)
            .ToHashSet();
    }
}
