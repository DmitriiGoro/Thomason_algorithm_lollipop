using ThomasonAlgorithm.Core.Graphs;

namespace ThomasonAlgorithm.Tests.LollipopAlgorithmTests;

public class LollipopAlgorithmTests
{
    private int[,] GetMatrixFromNeighborsString(string neighborsString)
    {
        var neighbors = neighborsString.Split('_');
        var matrix = new int[neighbors.Length, neighbors.Length];
        var cubicGraph = new CubicGraph(neighbors.Length);

        for (var i = 0; i < neighbors.Length; i++)
        {
            var currentNeighbors = neighbors[i].Split(',');
            foreach (var neib in currentNeighbors)
            {
                var neibInt = int.Parse(neib);
            
                matrix[i, neibInt] = 1;
            }
        }
        
        return matrix;
    }
}