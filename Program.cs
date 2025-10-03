using CarManager.Domain.DTOs;
using CarManager.Domain.Interfaces;
using CarManager.Domain.Services;
using CarManager.Infraestructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql
    (
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql")) 
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet(("/login"), () => ([FromBody] LoginDTO loginDTO, IAdministratorService service) =>
{
    if (service.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
});

app.Run();
