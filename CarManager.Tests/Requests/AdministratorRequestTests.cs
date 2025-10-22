using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CarManager.Domain.DTOs;
using CarManager.Domain.Entities;
using CarManager.Domain.ModelViews;
using CarManager.Tests.Helpers;

namespace CarManager.Tests.Requests;

[TestClass]
public class AdministratorRequestTests
{
    [TestClass]
    public class LoginTests
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
        public async Task ShouldAllowAdministratorLogin_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await Setup.client.PostAsync("/administrators/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Status code deve ser 200 OK");

            var json = await response.Content.ReadAsStringAsync();
            var loggedAdmin = JsonSerializer.Deserialize<AdministratorLoginResponse>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.IsNotNull(loggedAdmin, "A resposta não deve ser nula");
            Assert.AreEqual("adm@teste.com", loggedAdmin.Email);
            Assert.AreEqual("Adm", loggedAdmin.Role);
            Assert.IsFalse(string.IsNullOrWhiteSpace(loggedAdmin.Token), "O token JWT deve ser retornado");
        }

        [TestMethod]
        public async Task ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Email = "naoexiste@teste.com",
                Password = "senhaerrada"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await Setup.client.PostAsync("/administrators/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "Status code deve ser 401 Unauthorized");

            var json = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(string.IsNullOrEmpty(json) || json == "{}", "Resposta deve estar vazia ou sem corpo JSON");
        }

        [TestMethod]
        public async Task ShouldReturnBadRequest_WhenBodyIsInvalid()
        {
            // Arrange
            var invalidJson = "{ \"Email\": , }"; // JSON malformado
            var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            // Act
            var response = await Setup.client.PostAsync("/administrators/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Deve retornar 400 Bad Request");
        }
    }

    [TestClass]
    [DoNotParallelize]
    public class CreateTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext) => Setup.ClassInit(testContext);

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void ClassCleanup() => Setup.ClassCleanup();

        private static async Task<string> LoginAsAdminAsync()
        {
            var loginDto = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            var response = await Setup.client.PostAsync("/administrators/login", content);
            if (response.StatusCode != HttpStatusCode.OK)
                Assert.Fail($"Falha ao logar: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AdministratorLoginResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Token ?? throw new InvalidOperationException("Token não retornado no login.");
        }

        [TestInitialize]
        public async Task Initialize()
        {
            var token = await LoginAsAdminAsync();
            Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [TestMethod]
        public async Task ShouldCreateAdministrator_WhenDataIsValid()
        {
            // Arrange
            var newAdmin = new AdministratorDTO("novo@teste.com", "123456", "Adm");
            var content = new StringContent(JsonSerializer.Serialize(newAdmin), Encoding.UTF8, "application/json");

            // Act
            var response = await Setup.client.PostAsync("/administrators", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task ShouldReturnBadRequest_WhenBodyIsInvalid()
        {
            // Arrange
            var invalidAdmin = new { Email = "", Password = "", Role = "" };
            var content = new StringContent(JsonSerializer.Serialize(invalidAdmin), Encoding.UTF8, "application/json");

            // Act
            var response = await Setup.client.PostAsync("/administrators", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [TestClass]
    [DoNotParallelize]
    public class GetAllTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext) => Setup.ClassInit(testContext);

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void ClassCleanup() => Setup.ClassCleanup();

        private static async Task<string> LoginAsAdminAsync()
        {
            var loginDto = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            var response = await Setup.client.PostAsync("/administrators/login", content);
            if (response.StatusCode != HttpStatusCode.OK)
                Assert.Fail($"Falha ao logar: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AdministratorLoginResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Token ?? throw new InvalidOperationException("Token não retornado no login.");
        }

        [TestInitialize]
        public async Task Initialize()
        {
            var token = await LoginAsAdminAsync();
            Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [TestMethod]
        public async Task ShouldReturnAdministrators_WhenRequestIsValid()
        {
            // Arrange
            const int page = 1;

            // Act
            var response = await Setup.client.GetAsync($"/administrators?page={page}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Esperado retorno 200 OK ao buscar administradores.");

            var json = await response.Content.ReadAsStringAsync();
            var administrators = JsonSerializer.Deserialize<List<AdministratorResponse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(administrators, "A lista de administradores não deve ser nula.");
            Assert.IsTrue(administrators.Count > 0, "Deve retornar pelo menos um administrador cadastrado.");
            Assert.IsTrue(administrators.All(a => !string.IsNullOrWhiteSpace(a.Email)), "Todos os administradores devem ter e-mails válidos.");
        }
    }

    [TestClass]
    [DoNotParallelize]
    public class GetByIdTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext) => Setup.ClassInit(testContext);

        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
        public static void ClassCleanup() => Setup.ClassCleanup();

        private static async Task<string> LoginAsAdminAsync()
        {
            var loginDto = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");

            var response = await Setup.client.PostAsync("/administrators/login", content);
            if (response.StatusCode != HttpStatusCode.OK)
                Assert.Fail($"Falha ao logar: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AdministratorLoginResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Token ?? throw new InvalidOperationException("Token não retornado no login.");
        }

        [TestInitialize]
        public async Task Initialize()
        {
            var token = await LoginAsAdminAsync();
            Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [TestMethod]
        public async Task ShouldReturnAdministrator_WhenIdExists()
        {
            // Arrange
            const int existingId = 1;

            // Act
            var response = await Setup.client.GetAsync($"/administrators/{existingId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Esperado retorno 200 OK ao buscar administrador existente.");

            var json = await response.Content.ReadAsStringAsync();
            var administrator = JsonSerializer.Deserialize<AdministratorResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(administrator, "O administrador retornado não deve ser nulo.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(administrator.Email), "O e-mail do administrador deve ser válido.");
        }

        [TestMethod]
        public async Task ShouldReturnNotFound_WhenAdministratorDoesNotExist()
        {
            // Arrange
            const int nonExistentId = 9999;

            // Act
            var response = await Setup.client.GetAsync($"/administrators/{nonExistentId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Esperado retorno 404 NotFound para ID inexistente.");

            var message = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(message, "Administrator not found", "A mensagem deve indicar que o administrador não foi encontrado.");
        }
    }
}