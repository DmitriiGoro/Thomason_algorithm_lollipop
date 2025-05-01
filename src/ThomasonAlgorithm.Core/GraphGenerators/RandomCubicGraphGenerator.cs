namespace ThomasonAlgorithm.Core.GraphGenerator;

public static class RandomCubicGraphGenerator
{
    private static Random rand = new Random();

    /// <summary>
    /// Строит случайный 3-регулярный граф на n вершинах,
    /// гарантируя, что каждая хорда цикла имеет длину не более k.
    /// Возвращает null, если не удалось найти паросочетание после заданного числа попыток.
    /// </summary>
    public static CubicGraphLimitedChordLength Generate(int n, int kLow, int kUp, int maxAttempts = 100)
    {
        ValidateInput(n, kLow, kUp);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var graph = CreateInitialCycleGraph(n);
            var possibleNeighbors = GetPossibleNeighbors(n, kLow, kUp, graph);

            var matching = FindPerfectMatching(n, possibleNeighbors, graph);
            if (matching != null)
            {
                ApplyMatchingToGraph(graph, matching);
                if (graph.IsCubic()) return graph;
            }
        }

        return null;
    }

    // Проверка входных данных
    private static void ValidateInput(int n, int kLow, int kUp)
    {
        if (n < 4 || n % 2 != 0)
            throw new ArgumentException("3-регулярный граф возможен только при четном n >= 4.");
        if (kLow <= 1 || kUp > n / 2)
            throw new ArgumentException("Некорректное значение k.");
    }

    // Создание начального цикла
    private static CubicGraphLimitedChordLength CreateInitialCycleGraph(int n)
    {
        var graph = new CubicGraphLimitedChordLength(n);
        for (var i = 0; i < n; i++)
        {
            var j = (i + 1) % n;
            graph.AddEdge(i, j);
        }
        return graph;
    }

    // Получение допустимых соседей для хорды
    private static Dictionary<int, HashSet<int>> GetPossibleNeighbors(int n, int kLow, int kUp, CubicGraphLimitedChordLength graph)
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

                var dist = Math.Min(Math.Abs(j - i), n - Math.Abs(j - i));
                if (dist <= kUp && dist >= kLow)
                {
                    possibleNeighbors[i].Add(j);
                    possibleNeighbors[j].Add(i);
                }
            }
        }
        return possibleNeighbors;
    }

    // Применение найденного паросочетания
    private static void ApplyMatchingToGraph(CubicGraphLimitedChordLength graph, List<(int, int)> matching)
    {
        foreach (var (u, v) in matching)
        {
            graph.AdjacencyList[u].Add(v);
            graph.AdjacencyList[v].Add(u);
        }
    }
}
