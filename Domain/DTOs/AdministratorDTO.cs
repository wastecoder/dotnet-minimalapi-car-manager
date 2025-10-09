using CarManager.Domain.Enums;

namespace CarManager.Domain.DTOs;

public record AdministratorDTO (string Email, string Password, AdmRole Role);
