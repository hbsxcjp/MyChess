using Microsoft.EntityFrameworkCore;

namespace CChess;

public class Data : DbContext
{
    public DbSet<Ecco>? Eccos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string path = Path.Combine(Environment.CurrentDirectory, "data.db");
        optionsBuilder.UseSqlite($"Filename={path}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ecco>()
            .Property(e => e.Title).HasMaxLength(5).IsRequired();
    }
}

public class Ecco
{
    public int EccoId { get; }
    public string? Title { get; set; }
    public string? Name { get; set; }

}

// public class Manual
// {

// }
