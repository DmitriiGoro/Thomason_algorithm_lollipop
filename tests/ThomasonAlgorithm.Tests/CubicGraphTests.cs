using ThomasonAlgorithm.Core.Graphs;

namespace ThomasonAlgorithm.Tests;

public class CubicGraphTests
{
    [Theory]
    [MemberData(nameof(InvalidAdjacencyMatrices))]
    public void Test_WrongCubicGraphInitialization(int[,] invalidAdjacencyMatrix)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CubicGraph(invalidAdjacencyMatrix));
    }
    
    public static IEnumerable<object[]> InvalidAdjacencyMatrices =>
        new List<object[]>
        {
            new object[]
            {
                new int[,]
                {
                    {0, 1, 0, 1, 1},
                    {1, 0, 1, 0, 0},
                    {0, 1, 0, 1, 1},
                    {1, 0, 1, 0, 0},
                    {1, 0, 1, 0, 1}
                }
            },
            new object[]
            {
                new int[,]
                {
                    {0, 1, 0, 1},
                    {1, 0, 1, 0},
                    {0, 1, 0, 1},
                    {1, 0, 1, 0}
                }
            },
            new object[]
            {
                new int[,]
                {
                    {0, 1, 0, 1, 0, 0},
                    {1, 0, 1, 0, 0, 0},
                    {0, 1, 0, 1, 0, 0},
                    {1, 0, 1, 0, 0, 0},
                    {1, 0, 1, 0, 0, 0},
                    {1, 0, 1, 0, 0, 0}
                }
            }
        };
}