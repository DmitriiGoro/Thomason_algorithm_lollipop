using System.Reflection;
using ThomasonAlgorithm.Core.Algorithm;
using ThomasonAlgorithm.Core.GraphGenerators;
using ThomasonAlgorithm.Core.Graphs;
using ThomasonAlgorithm.Core.Models;

namespace ThomasonAlgorithm.Tests;

public class LollipopAlgorithmTests
{
        [Theory]
        [MemberData(nameof(CubicGraphsFromStringWithKnownStepsNumber))]
        public void Test_CheckKnownStepsNumber(string adjacencyList, int expectedStepsNumber)
        {
                var adjacencyMatrix = GetMatrixFromNeighborsString(adjacencyList);
                var graph = new CubicGraph(adjacencyMatrix);
                var graphAndCycle = new CubicGraphWithCycle(graph);
        
                Assert.NotNull(graph);
                Assert.True(graph.IsCubic());
        
                var steps = LollipopAlgorithm.FindSecondHamiltonianCycleAndReturnSteps(graphAndCycle);
        
                Assert.Equal(expectedStepsNumber, steps);
        }
        
        [Theory]
        [MemberData(nameof(RandomCubicGraphs))]
        public void Test_CheckLollipop(CubicGraph? graph)
        {
                var cubicGraphWithCycle = new CubicGraphWithCycle(graph!);
                var stepsNumber = LollipopAlgorithm.FindSecondHamiltonianCycleAndReturnSteps(cubicGraphWithCycle);
                
                Assert.True(stepsNumber > 1);
        }

        public static IEnumerable<object[]> RandomCubicGraphs()
                => new List<object[]>
                {
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphLimitedChords(30, 2, 10)
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphLimitedChords(100, 2, 9)
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphLimitedChords(50, 2, 15)
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphWithGapInChordLengths(50, 2, 15, new Gap(5, 9))
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphWithGapInChordLengths(100, 2, 20, new Gap(5, 10))
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphWithGapInChordLengths(200, 2, 40, new Gap(5, 10))
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphWithOddLengthChords(200, 2, 40)
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphWithOddLengthChords(100, 2, 39)
                        },
                        new object[]
                        {
                                RandomCubicGraphGenerator.GenerateGraphWithOddLengthChords(300, 10, 40)
                        },
                };

        public static IEnumerable<object[]> CubicGraphsFromStringWithKnownStepsNumber()
                => new List<object[]>
                {
                        new object[]
                        {
                                "1,6,19_0,2,16_1,3,17_2,4,9_3,5,7_4,6,10_0,5,7_4,6,8_7,9,13_3,8,10_5,9,11_10,12,18_11,13,15_8,12,14_13,15,19_12,14,16_1,15,17_2,16,18_11,17,19_0,14,18",
                                6
                        },
                        new object[]
                        {
                                "1,3,19_0,2,19_1,3,4_0,2,4_2,3,5_4,6,8_5,7,13_6,8,10_5,7,9_8,10,15_7,9,11_10,12,16_11,13,18_6,12,14_13,15,17_9,14,16_11,15,17_14,16,18_12,17,19_0,1,18",
                                3
                        },
                        new object[]
                        {
                                "1,10,15_0,2,3_1,3,13_1,2,4_3,5,8_4,6,15_5,7,14_6,8,12_4,7,9_8,10,11_0,9,11_9,10,12_7,11,13_2,12,14_6,13,15_0,5,14",
                                9
                        },
                        new object[]
                        {
                                "1,47,49_0,2,4_1,3,49_2,4,6_1,3,5_4,6,7_3,5,7_5,6,8_7,9,11_8,10,12_9,11,13_8,10,12_9,11,13_10,12,14_13,15,17_14,16,18_15,17,19_14,16,18_15,17,19_16,18,20_19,21,22_20,22,23_20,21,23_21,22,24_23,25,27_24,26,28_25,27,29_24,26,28_25,27,29_26,28,30_29,31,32_30,32,34_30,31,33_32,34,36_31,33,35_34,36,37_33,35,37_35,36,38_37,39,41_38,40,42_39,41,43_38,40,42_39,41,43_40,42,44_43,45,46_44,46,48_44,45,47_0,46,48_45,47,49_0,2,48",
                                26
                        },                                  
                        new object[]
                        {
                                "1,24,25_0,2,6_1,3,25_2,4,5_3,5,7_3,4,6_1,5,7_4,6,8_7,9,13_8,10,12_9,11,14_10,12,15_9,11,13_8,12,14_10,13,15_11,14,16_15,17,21_16,18,20_17,19,23_18,20,22_17,19,21_16,20,22_19,21,23_18,22,24_0,23,25_0,2,24",
                                28
                        },                        
                        new object[]
                        {
                                "1,31,35_0,2,32_1,3,35_2,4,6_3,5,9_4,6,8_3,5,7_6,8,12_5,7,9_4,8,10_9,11,15_10,12,14_7,11,13_12,14,16_11,13,15_10,14,16_13,15,17_16,18,22_17,19,23_18,20,24_19,21,25_20,22,26_17,21,23_18,22,24_19,23,25_20,24,26_21,25,27_26,28,30_27,29,33_28,30,34_27,29,31_0,30,32_1,31,33_28,32,34_29,33,35_0,2,34",
                                12
                        },
                        new object[]
                        {
                                "1,10,39_0,2,32_1,3,11_2,4,33_3,5,34_4,6,14_5,7,36_6,8,17_7,9,18_8,10,19_0,9,11_2,10,12_11,13,22_12,14,23_5,13,15_14,16,24_15,17,25_7,16,18_8,17,19_9,18,20_19,21,30_20,22,31_12,21,23_13,22,24_15,23,25_16,24,26_25,27,35_26,28,37_27,29,38_28,30,39_20,29,31_21,30,32_1,31,33_3,32,34_4,33,35_26,34,36_6,35,37_27,36,38_28,37,39_0,29,38",
                                6
                        },
                        new object[]
                        {
                                "1,6,39_0,2,8_1,3,9_2,4,10_3,5,11_4,6,12_0,5,7_6,8,13_1,7,9_2,8,10_3,9,11_4,10,12_5,11,13_7,12,14_13,15,20_14,16,22_15,17,23_16,18,24_17,19,25_18,20,26_14,19,21_20,22,28_15,21,23_16,22,24_17,23,25_18,24,26_19,25,27_26,28,34_21,27,29_28,30,35_29,31,36_30,32,37_31,33,38_32,34,39_27,33,35_29,34,36_30,35,37_31,36,38_32,37,39_0,33,38",
                                21
                        },
                        new object[]
                        {
                                "1,6,39_0,2,8_1,3,9_2,4,10_3,5,11_4,6,12_0,5,7_6,8,13_1,7,9_2,8,10_3,9,11_4,10,12_5,11,13_7,12,14_13,15,20_14,16,22_15,17,23_16,18,24_17,19,25_18,20,26_14,19,21_20,22,28_15,21,23_16,22,24_17,23,25_18,24,26_19,25,27_26,28,34_21,27,29_28,30,35_29,31,36_30,32,37_31,33,38_32,34,39_27,33,35_29,34,36_30,35,37_31,36,38_32,37,39_0,33,38",
                                21
                        },
                        new object[] // graph with gap in chords
                        {
                                "1,25,29_0,2,23_1,3,5_2,4,26_3,5,24_2,4,6_5,7,12_6,8,9_7,9,27_7,8,10_9,11,13_10,12,17_6,11,13_10,12,14_13,15,21_14,16,19_15,17,22_11,16,18_17,19,29_15,18,20_19,21,28_14,20,22_16,21,23_1,22,24_4,23,25_0,24,26_3,25,27_8,26,28_20,27,29_0,18,28",
                                7
                        },
                        new object[] 
                        {
                                "1,5,29_0,2,24_1,3,28_2,4,8_3,5,20_0,4,6_5,7,9_6,8,13_3,7,9_6,8,10_9,11,14_10,12,25_11,13,16_7,12,14_10,13,15_14,16,21_12,15,17_16,18,19_17,19,22_17,18,20_4,19,21_15,20,22_18,21,23_22,24,27_1,23,25_11,24,26_25,27,29_23,26,28_2,27,29_0,26,28",
                                6
                        },
                        new object[] 
                        {
                                "1,3,29_0,2,27_1,3,12_0,2,4_3,5,7_4,6,21_5,7,10_4,6,8_7,9,17_8,10,29_6,9,11_10,12,19_2,11,13_12,14,25_13,15,23_14,16,22_15,17,18_8,16,18_16,17,19_11,18,20_19,21,24_5,20,22_15,21,23_14,22,24_20,23,25_13,24,26_25,27,28_1,26,28_26,27,29_0,9,28",
                                11
                        },
                        new object[] 
                        {
                                "1,41,40_0,2,39_1,3,38_2,4,6_3,5,41_4,6,34_5,7,3_6,8,37_7,9,36_8,10,31_9,11,30_10,12,14_11,13,33_12,14,26_13,15,11_14,16,29_15,17,28_16,18,23_17,19,22_18,20,21_19,21,25_20,22,19_21,23,18_22,24,17_23,25,27_24,26,20_25,27,13_26,28,24_29,27,16_28,30,15_29,31,10_30,32,9_31,33,35_32,34,12_33,35,5_34,36,32_35,37,8_36,38,7_37,39,2_38,40,1_39,41,0_40,0,4",
                                3750
                        }
                };

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