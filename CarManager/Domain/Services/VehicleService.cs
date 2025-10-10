using CarManager.Domain.Entities;
using CarManager.Domain.Interfaces;
using CarManager.Infraestructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CarManager.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly DatabaseContext _context;
    public  VehicleService(DatabaseContext context)
    {
        _context = context;
    }

    public void Add(Vehicle vehicle)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public void Update(Vehicle vehicle)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }

    public void Delete(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public Vehicle? GetById(int id)
    {
        return _context.Vehicles.Find(id);
    }

    public List<Vehicle> GetAll(int? page, int? pageSize, string? name, string? brand)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(v => v.Name.ToLower().Contains(name.ToLower()));
        }
        
        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(v => v.Brand.ToLower().Contains(brand.ToLower()));
        }

        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        return query.ToList();
    }
}