namespace ThomasonAlgorithm.Tests.UnitTests;

public class CalculateChordLengthTest
{
    [Theory]
    [InlineData(2, 5, 20, 3)]
    [InlineData(18, 2, 20, 4)]
    [InlineData(0, 1, 6, 1)]
    [InlineData(0, 3, 6, 3)]
    [InlineData(0, 5, 6, 1)]
    [InlineData(4, 1, 6, 3)]
    public void GetChordLength_ReturnsExpectedResult(int i, int j, int n, int expected)
    {
        var result = GetChordLength(i, j, n);
        Assert.Equal(expected, result);
    }
    
    private static int GetChordLength(int i, int j, int n) => Math.Min(Math.Abs(j - i), n - Math.Abs(j - i));

}