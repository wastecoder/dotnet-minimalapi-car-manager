using System.Reflection;
using CarManager.Domain.Entities;
using CarManager.Domain.Enums;
using CarManager.Domain.Services;
using CarManager.Infraestructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CarManager.Tests.Domain.Services;

[TestClass]
public class AdministratorServiceTests
{
    private DatabaseContext CreateTestContext()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var context = new DatabaseContext(configuration);

        // context.Database.EnsureDeleted(); // Limpa o banco de dados antes de cada teste
        context.Database.EnsureCreated();

        return context;
    }

    [TestMethod]
    public void ShouldAllowSaveAdministrator()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrator
        {
            Id = 1,
            Email = "teste@teste.com",
            Password = "teste",
            Role = AdmRole.Adm
        };

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Add(adm);

        // Assert
        Assert.AreEqual(1, administratorService.GetAll(page: 1, pageSize: 5).Count());
    }

    [TestMethod]
    public void ShouldAllowGetAdministratorById()
    {
        // Arrange
        var context = CreateTestContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrator
        {
            Email = "teste@teste.com",
            Password = "teste",
            Role = AdmRole.Adm
        };

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Add(adm);
        var databaseAdministrator = administratorService.GetById(adm.Id);

        // Assert
        Assert.AreEqual(1, databaseAdministrator?.Id);
    }
}