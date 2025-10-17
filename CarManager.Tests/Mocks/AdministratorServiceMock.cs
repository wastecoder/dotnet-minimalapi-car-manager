using CarManager.Domain.DTOs;
using CarManager.Domain.Entities;
using CarManager.Domain.Enums;
using CarManager.Domain.Interfaces;

namespace CarManager.Tests.Mocks;

public class AdministratorServiceMock : IAdministratorService
{
    private static List<Administrator> administrators = new List<Administrator>()
    {
        new Administrator{
            Id = 1,
            Email = "adm@teste.com",
            Password = "123456",
            Role = AdmRole.Adm
        },
        new Administrator{
            Id = 2,
            Email = "editor@teste.com",
            Password = "123456",
            Role = AdmRole.Editor
        }
    };

    public Administrator? Login(LoginDTO loginDTO)
    {
        return administrators.Find(a => a.Email == loginDTO.Email
                                        && a.Password == loginDTO.Password);
    }

    public Administrator Add(Administrator administrator)
    {
        administrator.Id = administrators.Count + 1;
        administrators.Add(administrator);

        return administrator;
    }

    public Administrator? GetById(int id)
    {
        return administrators.Find(a => a.Id == id);
    }

    public List<Administrator> GetAll(int? page, int? pageSize)
    {
        return administrators;
    }
}