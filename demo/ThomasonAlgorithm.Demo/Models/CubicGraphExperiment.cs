using System.ComponentModel.DataAnnotations.Schema;

namespace ThomasonAlgorithm.Demo.Models;

public sealed record CubicGraphExperiment
{
    public CubicGraphExperiment(int verticesNumber, int kLow, int kUp, int maxChordLength, int lollipopStepsNumber, Dictionary<int, int> chordLengths, int[,] adjacencyMatrix)
    {
        VerticesNumber = verticesNumber;
        KLow = kLow;
        KUp = kUp;
        MaxChordLength = maxChordLength;
        LollipopStepsNumber = lollipopStepsNumber;
        ChordLengths = chordLengths;
        AdjacencyMatrix = adjacencyMatrix;
        Timestamp = DateTime.UtcNow;        
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// number of vertex in graph
    /// </summary>
    [Column("vertices_number")]
    public int VerticesNumber { get; set; }
    
    /// <summary>
    /// chord length lower limit 
    /// </summary>
    [Column("K_low")]
    public int KLow { get; set; }
    
    /// <summary>
    /// chord length upper limit
    /// </summary>
    [Column("K_up")]
    public int KUp { get; set; }
    
    /// <summary>
    /// biggest chord length in graph
    /// </summary>
    [Column("max_chord_length")]
    public int MaxChordLength { get; set; }
    
    /// <summary>
    /// number of steps which algorithm required to find second hamiltonian cycle in presented graph
    /// </summary>
    [Column("lollipop_steps_number")]
    public int LollipopStepsNumber { get; set; }

    /// <summary>
    /// chord lengths and its number which exist in graph
    /// </summary>
    [Column("chord_lengths", TypeName = "jsonb")]
    public Dictionary<int, int> ChordLengths { get; set; }
    
    /// <summary>
    /// NxN matrix 
    /// </summary>
    [Column("adjacency_matrix", TypeName = "jsonb")]
    public int[,] AdjacencyMatrix { get; set; }
    
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}