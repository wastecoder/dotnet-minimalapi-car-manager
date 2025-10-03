using CarManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarManager.Infraestructure.Database;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<Administrator> Administrators { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var connectionString = _configuration.GetConnectionString("MySql");
        if (!string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            );       
        }
    }
}
