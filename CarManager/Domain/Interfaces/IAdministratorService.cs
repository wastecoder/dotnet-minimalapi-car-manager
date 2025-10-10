using CarManager.Domain.DTOs;
using CarManager.Domain.Entities;

namespace CarManager.Domain.Interfaces;

public interface IAdministratorService
{
    Administrator? Login(LoginDTO loginDto);
    Administrator Add(Administrator administrator);
    Administrator? GetById(int id);
    List<Administrator> GetAll(int? page, int? pageSize);
}