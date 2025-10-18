using CarManager.Domain.DTOs;
using CarManager.Domain.Entities;
using CarManager.Domain.Interfaces;
using CarManager.Infraestructure.Database;

namespace CarManager.Domain.Services;

public class AdministratorService : IAdministratorService
{
    private readonly DatabaseContext _context;
    public AdministratorService(DatabaseContext context)
    {
        _context = context;
    }

    public Administrator? Login(LoginDTO loginDto)
    {
        return _context.Administrators.FirstOrDefault(a =>
            a.Email == loginDto.Email &&
            a.Password == loginDto.Password);
    }

    public Administrator Add(Administrator administrator)
    {
        if (_context.Administrators.Any(a => a.Email == administrator.Email))
            throw new InvalidOperationException("Email already registered.");

        _context.Administrators.Add(administrator);
        _context.SaveChanges();
        return administrator;
    }

    public Administrator? GetById(int id)
    {
        return _context.Administrators.FirstOrDefault(a => a.Id == id);
    }

    public List<Administrator> GetAll(int? page, int? pageSize)
    {
        var query = _context.Administrators.AsQueryable();

        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        return query.ToList();
    }
}