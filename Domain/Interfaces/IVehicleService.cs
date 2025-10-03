using CarManager.Domain.Entities;

namespace CarManager.Domain.Interfaces;

public interface IVehicleService
{
    void Add(Vehicle vehicle);
    void Update(Vehicle vehicle);
    void Delete(Vehicle vehicle);
    Vehicle? GetById(int id);
    List<Vehicle> GetAll(int? page, int? pageSize, string? name, string? brand);
}