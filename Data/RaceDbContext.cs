// Data/RaceDbContext.cs
using Microsoft.EntityFrameworkCore;

public class RaceDbContext : DbContext
{
    public RaceDbContext(DbContextOptions<RaceDbContext> options) : base(options)
    {
    }

    public DbSet<Race> Races { get; set; }
}
