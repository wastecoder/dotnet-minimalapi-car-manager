using CarManager.Domain.DTOs;
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
    public void Login_ShouldReturnAdministrator_WhenCredentialsAreValid()
    {
        // Arrange
        using var context = CreateTestContext();
        var adm = new Administrator
        {
            Email = "adm@teste.com",
            Password = "123456",
            Role = AdmRole.Adm
        };
        var service = new AdministratorService(context);
        service.Add(adm);

        var loginDto = new LoginDTO
        {
            Email = "adm@teste.com",
            Password = "123456"
        };

        // Act
        var result = service.Login(loginDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(adm.Email, result.Email);
        Assert.AreEqual(adm.Password, result.Password);
        Assert.AreEqual(AdmRole.Adm, result.Role);
    }

    [TestMethod]
    public void Login_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        using var context = CreateTestContext();
        var service = new AdministratorService(context);

        var loginDto = new LoginDTO
        {
            Email = "invalido@teste.com",
            Password = "errada"
        };

        // Act
        var result = service.Login(loginDto);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Add_ShouldAddAdministrator_WhenDataIsValid()
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
        var allAdmins = administratorService.GetAll(page: 1, pageSize: 5);
        Assert.IsNotNull(allAdmins);
        Assert.AreEqual(1, allAdmins.Count);

        var savedAdmin = allAdmins.First();
        Assert.AreEqual("teste@teste.com", savedAdmin.Email);
        Assert.AreEqual("teste", savedAdmin.Password);
        Assert.AreEqual(AdmRole.Adm, savedAdmin.Role);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Add_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        using var context = CreateTestContext();
        var service = new AdministratorService(context);
        var adm1 = new Administrator
        {
            Email = "duplicado@teste.com",
            Password = "123456",
            Role = AdmRole.Adm
        };
        var adm2 = new Administrator
        {
            Email = "duplicado@teste.com",
            Password = "outra",
            Role = AdmRole.Adm
        };

        service.Add(adm1);

        // Act
        service.Add(adm2); // Deve lançar InvalidOperationException
    }

    [TestMethod]
    public void GetById_ShouldReturnAdministrator_WhenIdIsValid()
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

        Assert.IsNotNull(savedAdministrator);
        Assert.AreEqual(generatedId, savedAdministrator.Id);
        Assert.AreEqual("teste2@teste.com", savedAdministrator.Email);
    }

    [TestMethod]
    public void GetById_ShouldReturnNull_WhenAdministratorDoesNotExist()
    {
        // Arrange
        using var context = CreateTestContext();
        const int missingId = 999;
        var service = new AdministratorService(context);

        // Act
        var result = service.GetById(missingId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_ShouldReturnTwoAdministrators_WhenTwoAreSaved()
    {
        // Arrange
        using var context = CreateTestContext();
        var service = new AdministratorService(context);

        service.Add(new Administrator
        {
            Email = "adm1@teste.com",
            Password = "123",
            Role = AdmRole.Adm
        });

        service.Add(new Administrator
        {
            Email = "adm2@teste.com",
            Password = "456",
            Role = AdmRole.Adm
        });

        // Act
        var result = service.GetAll(page: 1, pageSize: 5);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(a => a.Email == "adm1@teste.com"));
        Assert.IsTrue(result.Any(a => a.Email == "adm2@teste.com"));
    }
}