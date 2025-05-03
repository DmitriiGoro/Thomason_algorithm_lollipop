using Microsoft.EntityFrameworkCore;
using ThomasonAlgorithm.Demo.Models;

namespace ThomasonAlgorithm.Demo;

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
        
        modelBuilder.Entity<CubicGraphExperiment>()
            .Property(x => x.AdjacencyMatrix)
            .HasColumnType("jsonb");
    } 
}