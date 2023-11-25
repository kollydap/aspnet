// Data/DriverDbContext.cs
using Microsoft.EntityFrameworkCore;

public class DriverDbContext : DbContext
{
    public DriverDbContext(DbContextOptions<DriverDbContext> options) : base(options)
    {
    }

    public DbSet<Driver> Drivers { get; set; }
}
