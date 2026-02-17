using Backend.Models.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Database;

public class DataContext : DbContext
{
    public DbSet<Item> Items => Set<Item>();

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(AppContext.BaseDirectory, "database.db");

        optionsBuilder.UseSqlite($"DataSource={dbPath}");
    }
}
