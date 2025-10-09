using CarManager.Domain.Enums;

namespace CarManager.Domain.ModelViews;

public record AdministratorResponse (int id, string Email, AdmRole Role);
