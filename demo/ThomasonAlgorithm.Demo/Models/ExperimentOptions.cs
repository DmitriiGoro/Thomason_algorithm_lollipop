namespace ThomasonAlgorithm.Demo.Models;

public sealed record ExperimentOptions{

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
        
        if (startVerticesNumber % 2 != 0)
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