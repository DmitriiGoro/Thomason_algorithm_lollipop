using System.Collections.Generic;
using Xunit;
namespace ThomasonAlgorithm.Tests.UnitTests;

public class ComputeChordLengthsFromAdjacencyMatrix
{
    [Theory]
    [MemberData(nameof(AdjacencyMatrix))]
    private void ComputeChordLengths(int[,] adjacencyMatrix, int expectedMaxChordLength, Dictionary<int, int> expectedChordLengths)
    {
        var n = adjacencyMatrix.GetLength(0);
        var chordLengths = new Dictionary<int, int>();
        var maxChordLength = -1;

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (adjacencyMatrix[i, j] == 1)
                {
                    var chordLength = Math.Min(Math.Abs(i - j), n - Math.Abs(i - j));

                    if (chordLength <= 1)
                        continue;

                    if (!chordLengths.TryAdd(chordLength, 1))
                    {
                        chordLengths[chordLength]++;
                    }

                    if (chordLength > maxChordLength)
                    {
                        maxChordLength = chordLength;
                    }
                }
            }
        }
        
        Assert.Equal(expectedMaxChordLength, maxChordLength);
        Assert.Equal(expectedChordLengths, chordLengths);
    }

    public static IEnumerable<object[]> AdjacencyMatrix => new List<object[]>
    {
        new object[]
        {
            new int[,]
            {
                {0, 1, 0, 1, 0, 1},
                {1, 0, 1, 0, 0, 1},
                {0, 1, 0, 1, 1, 0},
                {1, 0, 1, 0, 1, 0},
                {0, 0, 1, 1, 0, 1},
                {1, 1, 0, 0, 1, 0}
            },
            3,
            new Dictionary<int, int> { [2] = 2, [3] = 1 }
        },
        new object[]
        {
            new int[,]
            {
                {0, 1, 1, 1},
                {1, 0, 1, 1},
                {1, 1, 0, 1},
                {1, 1, 1, 0}
            },
            2,
            new Dictionary<int, int> { [2] = 2 }
        },
        new object[]
        {
            new int[,]
            {
                {0, 1, 0, 0, 0, 1, 0, 0, 0, 1},
                {1, 0, 1, 0, 0, 0, 0, 0, 0, 1},
                {0, 1, 0, 1, 0, 0, 0, 0, 1, 0},
                {0, 0, 1, 0, 1, 0, 0, 1, 0, 0},
                {0, 0, 0, 1, 0, 1, 1, 0, 0, 0},
                {1, 0, 0, 0, 1, 0, 1, 0, 0, 0},
                {0, 0, 0, 0, 1, 1, 0, 1, 0, 0},
                {0, 0, 0, 1, 0, 0, 1, 0, 1, 0},
                {0, 0, 1, 0, 0, 0, 0, 1, 0, 1},
                {1, 1, 0, 0, 0, 0, 0, 0, 1, 0}
            },
            5,
            new Dictionary<int, int> { [2] = 2, [4] = 2, [5] = 1 }
        }
    };
}
