using ThomasonAlgorithm.Core.Extensions;
using ThomasonAlgorithm.Core.GraphGenerators;
using ThomasonAlgorithm.Core.Graphs;

namespace ThomasonAlgorithm.Tests;

public class CubicGraphWithCycleTest
{
    [Theory]
    [MemberData(nameof(RandomCubicGraphs))]
    public void Test_GeneratingCommonCycle(CubicGraph graph, HashSet<int> hamiltonianCycle)
    {
        var cubicGraphWithCycle = new CubicGraphWithCycle(graph);
        Assert.NotNull(cubicGraphWithCycle.HamiltonianCycle);
        
        var sequencedHamiltonianCycle = GetHamiltonianCycleSequencedVertices(cubicGraphWithCycle.HamiltonianCycle);
        Assert.Equal(hamiltonianCycle, sequencedHamiltonianCycle);
    }
    
    public static IEnumerable<object[]> RandomCubicGraphs =>
        new List<object[]>
        {
            new object[]
            {
                RandomCubicGraphGenerator.GenerateGraphLimitedChords(20, 2, 5), new HashSet<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19}
            },
            new object[]
            {
                RandomCubicGraphGenerator.GenerateGraphLimitedChords(26, 2, 5), new HashSet<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25}
            },
            new object[]
            {
                RandomCubicGraphGenerator.GenerateGraphLimitedChords(30, 2, 7), new HashSet<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29}
            }
        };
    
    private static HashSet<int> GetHamiltonianCycleSequencedVertices(Dictionary<int, List<int>> hamiltonianCycle)
    {
        var sequence = new HashSet<int>();
        var currentVertex = 0;

        while (!sequence.Contains(currentVertex))
        {
            sequence.Add(currentVertex);

            var currentVertexNeighborsInCycle = hamiltonianCycle[currentVertex];
            var nextVertex = currentVertexNeighborsInCycle.FirstPossibleNeighborOrNull(x => !sequence.Contains(x));
 
            currentVertex = nextVertex ?? currentVertex;
        }
        
        return sequence;        
    }
}