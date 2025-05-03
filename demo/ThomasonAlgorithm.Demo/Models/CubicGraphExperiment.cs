using System.ComponentModel.DataAnnotations.Schema;

namespace ThomasonAlgorithm.Demo.Models;

public sealed record CubicGraphExperiment
{
    public CubicGraphExperiment(int verticesNumber, int kLow, int kUp, int lollipopStepsNumber, string chordLengthsString, string adjacencyMatrix)
    {
        VerticesNumber = verticesNumber;
        KLow = kLow;
        KUp = kUp;
        LollipopStepsNumber = lollipopStepsNumber;
        ChordLengthsString = chordLengthsString;
        AdjacencyMatrix = adjacencyMatrix;
        Timestamp = DateTime.UtcNow;        
    }
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// количество вершин графа
    /// </summary>
    [Column("vertices_number")]
    public int VerticesNumber { get; set; }
    
    /// <summary>
    /// максимальнодопустимая длина хорды в графе
    /// </summary>
    [Column("K_low")]
    public int KLow { get; set; }
    
    /// <summary>
    /// фактически максимальная длина хорды
    /// </summary>
    [Column("K_up")]
    public int KUp { get; set; }
    
    /// <summary>
    /// время поиска второго гамильтонова цикла
    /// </summary>
    [Column("lollipop_steps_number")]
    public int LollipopStepsNumber { get; set; }

    // строка пар длина хорды:количество
    [Column("chord_lengths_string")]
    public string ChordLengthsString { get; set; }
    
    // строка пар длина хорды:количество
    [Column("adjacency_matrix")]
    public string AdjacencyMatrix { get; set; }
    
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}