# Cubic Graph Experiment Demo
**See also**:
- [Getting Started](getting-started.md)
- [API Reference](./api/toc.html)

# Cubic Graph Experiment Demo

## Overview
This demonstration showcases how to use ThomasonAlgorithm.Core to:
- Generate random cubic graphs with specified constraints
- Find Hamiltonian cycles using the Lollipop algorithm
- Store and analyze experiment results in PostgreSQL

## Prerequisites
- .NET 8+ SDK
- PostgreSQL 14+ with JSONB support
- NuGet packages:
  ```bash
  dotnet add package Npgsql
  dotnet add package Microsoft.EntityFrameworkCore
  dotnet add package ThomasonAlgorithm.Core

# 1. Data Model Implementation 
## 1.1 Entity Definition
```csharp
public sealed record CubicGraphExperiment
{
    public CubicGraphExperiment(int verticesNumber, int kLow, int kUp, int maxChordLength, int lollipopStepsNumber, Dictionary<int, int> chordLengths)
    {
        VerticesNumber = verticesNumber;
        KLow = kLow;
        KUp = kUp;
        MaxChordLength = maxChordLength;
        LollipopStepsNumber = lollipopStepsNumber;
        ChordLengths = chordLengths;
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
    
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}
```

## 1.2 Database Context

```csharp
public class AppDbContext : DbContext
{
    public DbSet<CubicGraphExperiment> CubicGraphExperiments { get; set; } 
        
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CubicGraphExperiment>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<CubicGraphExperiment>()
            .Property(x => x.ChordLengths)
            .HasColumnType("jsonb");
    } 
}
```

# 2. Infrastructure Setup
## 2.1 Database Configuration

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddDbInfrastructure(this IServiceCollection services, string connectionString)
    {
        if (connectionString == null)
            throw new ArgumentNullException(nameof(connectionString), "Connection string is required");

        var npgSqlDatasourceBuilder = new NpgsqlDataSourceBuilder(connectionString).EnableDynamicJson().Build();
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(npgSqlDatasourceBuilder).UseSnakeCaseNamingConvention());

        return services;
    }
}
```
## 2.2 Service Registration
```csharp
// In Program.cs
builder.Services.AddDbInfrastructure("Host=localhost;Database=graph_db;Username=postgres;Password=your_password");
````

# 3. Experiment Configuration
## 3.1 Parameter Validation
```csharp
public sealed record ExperimentOptions {
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
```
# 4. Core Experiment Service
## 4.1 Implementation

```csharp
public class CubicGraphExperimentService
{
    public async Task ExperimentCubicGraph(ExperimentOptions options, IHost app)
    {
        var (maxN, kLow, kUp, startVerticesNumber, maxAttempt, maxGraphsSameSize) = options;
        
        const int batchSize = 1000;
        const int initialStringBuilderCapacity = 256;

        var allExperiments = new List<CubicGraphExperiment>(batchSize);

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        for (var n = startVerticesNumber; n <= maxN; n += 2)
        {
            var experiments = new List<CubicGraphExperiment>(maxGraphsSameSize);
            await Parallel.ForEachAsync(Enumerable.Range(0, maxGraphsSameSize), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (graphsSameSize, ct) =>
            {
                var stringBuilder = new StringBuilder(initialStringBuilderCapacity);

                var randomCubicGraph = RandomCubicGraphGenerator.GenerateGraphLimitedChords(n, kLow, kUp, maxAttempt);

                if (randomCubicGraph == null)
                {
                    lock (Console.Out)
                    {
                        Console.WriteLine("Cubic graph was not generated.");
                    }
                    return;
                }

                var cubicGraphWithCycle = new CubicGraphWithCycle(randomCubicGraph);
                var lollipopStepsNumber = LollipopAlgorithm.FindSecondHamiltonianCycleAndReturnSteps(cubicGraphWithCycle);

                stringBuilder.AppendFormat("lollipop_steps_number: {0}, for graph with n: {1}, kLow: {2}, kUp: {3}", lollipopStepsNumber, n, kLow, kUp);
                lock (Console.Out)
                {
                    Console.WriteLine(stringBuilder.ToString());
                }
                stringBuilder.Clear();


                var experiment = new CubicGraphExperiment(n, kLow, kUp, randomCubicGraph.MaxChordLength, lollipopStepsNumber, randomCubicGraph.ChordsLengths);
                lock (experiments)
                {
                    experiments.Add(experiment);
                }
            });

            allExperiments.AddRange(experiments);

            if (allExperiments.Count >= batchSize)
            {
                await dbContext.BulkInsertAsync(allExperiments);
                allExperiments.Clear();
            }
        }

        if (allExperiments.Any())
        {
            await dbContext.BulkInsertAsync(allExperiments);
        }
    }
}
```

# 5. Execution Example
## Running the Experiment

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbInfrastructure(builder.Configuration.GetConnectionString("Default"));

var app = builder.Build();

// Configure experiment parameters
var options = new ExperimentOptions(
    maxN: 1000,
    kLow: 2,
    kUp: 10,
    maxAttempt: 100,
    graphSameSize: 500);

// Execute
var experimentService = new CubicGraphExperimentService();
await experimentService.RunExperiment(options, app);
```
