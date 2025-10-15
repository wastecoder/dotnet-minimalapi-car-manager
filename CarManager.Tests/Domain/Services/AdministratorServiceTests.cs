using CarManager.Domain.Entities;
using CarManager.Domain.Enums;
using CarManager.Domain.Services;
using CarManager.Infraestructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CarManager.Tests.Domain.Services;

[TestClass]
public class AdministratorServiceTests
{
    private DatabaseContext CreateTestContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new DatabaseContext(options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }

    [TestMethod]
    public void ShouldAllowSaveAdministrator()
    {
        // Arrange
        using var context = CreateTestContext();
        var adm = new Administrator
        {
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
        using var context = CreateTestContext();
        var adm = new Administrator
        {
            Email = "teste2@teste.com",
            Password = "teste",
            Role = AdmRole.Adm
        };

        var administratorService = new AdministratorService(context);

        administratorService.Add(adm);

        var generatedId = adm.Id;
        var savedAdministrator = administratorService.GetById(generatedId);

        Assert.AreEqual(generatedId, savedAdministrator?.Id);
    }
}