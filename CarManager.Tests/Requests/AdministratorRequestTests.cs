using System.Net;
using System.Text;
using System.Text.Json;
using CarManager.Domain.DTOs;
using CarManager.Domain.ModelViews;
using CarManager.Tests.Helpers;

namespace CarManager.Tests.Requests;

[TestClass]
public class AdministratorRequestTests
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task Should_Allow_Administrator_Login_When_Credentials_Are_Valid()
    {
        // Arrange
        var loginDto = new LoginDTO{
            Email = "adm@teste.com",
            Password = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8,  "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/administrators/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var loggedAdmin = JsonSerializer.Deserialize<AdministratorLoginResponse>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(loggedAdmin);
        Assert.IsNotNull(loggedAdmin.Email);
        Assert.IsNotNull(loggedAdmin.Role);
        Assert.IsNotNull(loggedAdmin.Token);
    }
}