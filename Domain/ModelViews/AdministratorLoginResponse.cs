namespace CarManager.Domain.ModelViews;

public record AdministratorLoginResponse (string Email, string Role, string Token);
