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
    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator {
                Id = 1,
                Email = "administrador@teste.com",
                Password = "123456",
                Role = "Adm"
            }
        );
    }

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
