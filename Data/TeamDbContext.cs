// Data/DriverDbContext.cs
using Microsoft.EntityFrameworkCore;

public class TeamDbContext : DbContext
{
    public TeamDbContext(DbContextOptions<TeamDbContext> options) : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; }
}
