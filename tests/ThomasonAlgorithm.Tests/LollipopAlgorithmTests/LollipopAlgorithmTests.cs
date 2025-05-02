using ThomasonAlgorithm.Core.Algorithm;
using ThomasonAlgorithm.Core.Graphs;

namespace ThomasonAlgorithm.Tests.LollipopAlgorithmTests;

public class LollipopAlgorithmTests
{
        [Theory]
        [InlineData(
                "1,6,19_0,2,16_1,3,17_2,4,9_3,5,7_4,6,10_0,5,7_4,6,8_7,9,13_3,8,10_5,9,11_10,12,18_11,13,15_8,12,14_13,15,19_12,14,16_1,15,17_2,16,18_11,17,19_0,14,18",
                6)]
        [InlineData(
                "1,3,19_0,2,19_1,3,4_0,2,4_2,3,5_4,6,8_5,7,13_6,8,10_5,7,9_8,10,15_7,9,11_10,12,16_11,13,18_6,12,14_13,15,17_9,14,16_11,15,17_14,16,18_12,17,19_0,1,18",
                3)]        
        [InlineData(
                "1,10,15_0,2,3_1,3,13_1,2,4_3,5,8_4,6,15_5,7,14_6,8,12_4,7,9_8,10,11_0,9,11_9,10,12_7,11,13_2,12,14_6,13,15_0,5,14",
                9)]
        public void Test_GenerateGraphLimitedChords_ChordLengthsWithinBounds(string adjaencyList,
                int expectedStepsNumber)
        {
                var adjacencyMatrix = GetMatrixFromNeighborsString(adjaencyList);
                var graph = new CubicGraph(adjacencyMatrix);
                var graphAndCycle = new CubicGraphWithCycle(graph);

                Assert.NotNull(graph);
                Assert.True(graph.IsCubic());

                var steps = LollipopAlgorithm.FindSecondHamiltonianCycleAndReturnSteps(graphAndCycle);

                Assert.Equal(expectedStepsNumber, steps);
        }

        private int[,] GetMatrixFromNeighborsString(string neighborsString)
        {
                var neighbors = neighborsString.Split('_');
                var matrix = new int[neighbors.Length, neighbors.Length];

                for (var i = 0; i < neighbors.Length; i++)
                {
                        foreach (var neib in neighbors[i].Split(','))
                        {
                                matrix[i, int.Parse(neib)] = 1;
                        }
                }

                return matrix;
        }
}