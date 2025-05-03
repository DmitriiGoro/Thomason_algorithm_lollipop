namespace ThomasonAlgorithm.Demo.Models;

/// <summary>
/// This record holds the configuration options for running experiments on cubic graphs.
/// It contains parameters that define the behavior and settings for the graph generation and analysis.
/// </summary>
public sealed record ExperimentOptions{

    /// <summary>
    /// Initializes a new instance of the <see cref="ExperimentOptions"/> record with the specified parameters.
    /// </summary>
    /// <param name="maxN">The maximum number of vertices in the generated graphs.</param>
    /// <param name="kLow">The lower bound for the chord length in the graph.</param>
    /// <param name="kUp">The upper bound for the chord length in the graph.</param>
    /// <param name="maxAttempt">The maximum number of attempts to generate a cubic graph. Defaults to 100.</param>
    /// <param name="graphSameSize">The number of graphs with the same size to generate. Defaults to 100.</param>
    /// <param name="startVerticesNumber">The starting number of vertices. If not provided, it defaults to 2 * kUp.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any of the following conditions is violated:
    /// <list type="bullet">
    /// <item><description>maxN must be greater than or equal to 2 * kUp</description></item>
    /// <item><description>startVerticesNumber must be greater than or equal to 2 * kUp, and if provided, must be even</description></item>
    /// <item><description>maxN must be greater than or equal to startVerticesNumber</description></item>
    /// <item><description>kUp must be greater than or equal to kLow</description></item>
    /// <item><description>maxAttempt must be greater than 0</description></item>
    /// <item><description>graphSameSize must be greater than 0</description></item>
    /// </list>
    /// </exception>
    public ExperimentOptions(
        int maxN,
        int kLow,
        int kUp,
        int maxAttempt = 100,
        int graphSameSize = 100,
        int startVerticesNumber = -1)
    {
        if (maxN < 2 * kUp)
            throw new ArgumentOutOfRangeException(nameof(maxN), "maxN must be greater than or equal to 2 * kUp");
        
        if (startVerticesNumber != -1 && startVerticesNumber < 2 * kUp)
            throw new ArgumentOutOfRangeException(nameof(startVerticesNumber), "startVerticesNumber must be greater than or equal to 2 * kUp");
        
        if (startVerticesNumber != -1 && startVerticesNumber % 2 != 0)
            throw new ArgumentOutOfRangeException(nameof(startVerticesNumber), "startVerticesNumber must be even");
        
        if (maxN < startVerticesNumber)
            throw new ArgumentOutOfRangeException(nameof(maxN), "maxN must be greater than or equal to startVerticesNumber");
        
        if (kUp < kLow)
            throw new ArgumentOutOfRangeException(nameof(kUp), "kUp must be greater than or equal to kLow");
        
        if (maxAttempt <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxAttempt), "maxAttempt must be greater than 0");
        
        if (graphSameSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(graphSameSize), "graphSameSize must be greater than 0");

        MaxN = maxN;
        KLow = kLow;
        KUp = kUp;
        StartVerticesNumber = startVerticesNumber == -1 ? 2 * kUp : startVerticesNumber;
        MaxAttempt = maxAttempt;
        GraphSameSize = graphSameSize;
    }
    
    public void Deconstruct(
        out int maxN,
        out int kLow,
        out int kUp,
        out int startVerticesNumber,
        out int maxAttempt,
        out int graphSameSize)
    {
        maxN = MaxN;
        kLow = KLow;
        kUp = KUp;
        startVerticesNumber = StartVerticesNumber;
        maxAttempt = MaxAttempt;
        graphSameSize = GraphSameSize;
    }
    
    public int MaxN { get; init; }
    
    public int KLow { get; init; }
    
    public int KUp { get; init; }
    
    public int StartVerticesNumber { get; init; }
    
    public int MaxAttempt { get; init; }
    
    public int GraphSameSize { get; init; }
}