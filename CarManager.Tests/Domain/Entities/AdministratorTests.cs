using CarManager.Domain.Entities;
using CarManager.Domain.Enums;

namespace CarManager.Tests.Domain.Entities;

[TestClass]
public class AdministratorTests
{
    [TestMethod]
    public void ShouldAllowSetAdministratorProperties()
    {
        // Arrange & Act
        var adm = new Administrator
        {
            Id = 1,
            Email = "teste@teste.com",
            Password = "teste",
            Role = AdmRole.Adm
        };

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Password);
        Assert.AreEqual(AdmRole.Adm, adm.Role);
    }
}