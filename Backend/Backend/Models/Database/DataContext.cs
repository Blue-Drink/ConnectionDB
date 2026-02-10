using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Database;

public class DataContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"DataSource=database.db");
    }
}
