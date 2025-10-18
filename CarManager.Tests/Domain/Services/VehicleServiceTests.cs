using Microsoft.EntityFrameworkCore;
using CarManager.Domain.Entities;
using CarManager.Domain.Services;
using CarManager.Infraestructure.Database;

namespace CarManager.Tests.Domain.Services;

[TestClass]
public class VehicleServiceTests
{
    private static DatabaseContext CreateTestContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new DatabaseContext(options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }

    [TestClass]
    public class AddTests
    {
        [TestMethod]
        public void ShouldAddVehicle_WhenDataIsValid()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);
            var vehicle = new Vehicle
            {
                Name = "Carro 1",
                Brand = "Marca A"
            };

            // Act
            service.Add(vehicle);

            // Assert
            var allVehicles = service.GetAll(page: 1, pageSize: 5, name: null, brand: null);
            Assert.IsNotNull(allVehicles);
            Assert.AreEqual(1, allVehicles.Count);

            var savedVehicle = allVehicles.First();
            Assert.AreEqual("Carro 1", savedVehicle.Name);
            Assert.AreEqual("Marca A", savedVehicle.Brand);
        }

        [TestMethod]
        public void ShouldThrowException_WhenVehicleIsNull()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);

            // Act & Assert
            var ex = Assert.ThrowsException<ArgumentNullException>(() => service.Add(null!));
            Assert.AreEqual("Value cannot be null. (Parameter 'vehicle')", ex.Message);
        }
    }

    [TestClass]
    public class UpdateTests
    {
        [TestMethod]
        public void ShouldUpdateVehicle_WhenDataIsValid()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);
            var vehicle = new Vehicle
            {
                Name = "Carro Original",
                Brand = "Marca A"
            };
            service.Add(vehicle);

            vehicle.Name = "Carro Atualizado";
            vehicle.Brand = "Marca B";

            // Act
            service.Update(vehicle);

            // Assert
            var updated = service.GetById(vehicle.Id);
            Assert.IsNotNull(updated);
            Assert.AreEqual("Carro Atualizado", updated.Name);
            Assert.AreEqual("Marca B", updated.Brand);
        }

        [TestMethod]
        public void ShouldThrowException_WhenVehicleIsNull_OnUpdate()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);

            // Act & Assert
            var ex = Assert.ThrowsException<ArgumentNullException>(() => service.Update(null!));
            Assert.AreEqual("Value cannot be null. (Parameter 'vehicle')", ex.Message);
        }
    }

    [TestClass]
    public class DeleteTests
    {
        [TestMethod]
        public void ShouldDeleteVehicle_WhenItExists()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);
            var vehicle = new Vehicle
            {
                Name = "Carro a Deletar",
                Brand = "Marca X"
            };
            service.Add(vehicle);

            // Act
            service.Delete(vehicle);

            // Assert
            var result = service.GetById(vehicle.Id);
            Assert.IsNull(result);
        }
    }

    [TestClass]
    public class GetByIdTests
    {
        [TestMethod]
        public void ShouldReturnVehicle_WhenIdIsValid()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);
            var vehicle = new Vehicle
            {
                Name = "Carro Detalhe",
                Brand = "Marca Y"
            };
            service.Add(vehicle);

            // Act
            var result = service.GetById(vehicle.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(vehicle.Id, result.Id);
            Assert.AreEqual("Carro Detalhe", result.Name);
            Assert.AreEqual("Marca Y", result.Brand);
        }

        [TestMethod]
        public void ShouldReturnNull_WhenVehicleDoesNotExist()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);

            // Act
            var result = service.GetById(999);

            // Assert
            Assert.IsNull(result);
        }
    }

    [TestClass]
    public class GetAllTests
    {
        [TestMethod]
        public void ShouldReturnTwoVehicles_WhenTwoAreSaved()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);

            service.Add(new Vehicle
            {
                Name = "Carro 1",
                Brand = "Marca A"
            });
            service.Add(new Vehicle
            {
                Name = "Carro 2",
                Brand = "Marca B"
            });

            // Act
            var result = service.GetAll(page: 1, pageSize: 5, name: null, brand: null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(v => v.Name == "Carro 1"));
            Assert.IsTrue(result.Any(v => v.Name == "Carro 2"));
        }

        [TestMethod]
        public void ShouldFilterByName_WhenNameIsProvided()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);

            service.Add(new Vehicle
            {
                Name = "Uno",
                Brand = "Fiat"
            });
            service.Add(new Vehicle
            {
                Name = "Palio",
                Brand = "Fiat"
            });

            // Act
            var result = service.GetAll(page: 1, pageSize: 5, name: "Uno", brand: null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Uno", result.First().Name);
        }

        [TestMethod]
        public void ShouldFilterByBrand_WhenBrandIsProvided()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);

            service.Add(new Vehicle
            {
                Name = "Gol",
                Brand = "VW"
            });
            service.Add(new Vehicle
            {
                Name = "Uno",
                Brand = "Fiat"
            });

            // Act
            var result = service.GetAll(page: 1, pageSize: 5, name: null, brand: "Fiat");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Fiat", result.First().Brand);
        }

        [TestMethod]
        public void ShouldApplyPagination_WhenPageAndPageSizeAreProvided()
        {
            // Arrange
            using var context = CreateTestContext();
            var service = new VehicleService(context);

            for (var i = 1; i <= 10; i++)
            {
                service.Add(new Vehicle
                {
                    Name = $"Carro {i}",
                    Brand = "Marca A"
                });
            }

            // Act
            var result = service.GetAll(page: 2, pageSize: 3, name: null, brand: null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Carro 4", result.First().Name);
        }
    }
}