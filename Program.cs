using CarManager.Domain.DTOs;
using CarManager.Domain.Interfaces;
using CarManager.Domain.ModelViews;
using CarManager.Domain.Services;
using CarManager.Infraestructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql
    (
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql")) 
    );
});

var app = builder.Build();
#endregion

#region Home
// app.MapGet("/", () => Results.Json(new Home()));
app.MapGet("/", () =>
{
    Console.WriteLine("Accessing API documentation...");
    return Results.Redirect("/swagger");
});
#endregion

#region Administrators
app.MapGet(("/administrators/login"), () => ([FromBody] LoginDTO loginDTO, IAdministratorService service) =>
{
    if (service.Login(loginDTO) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
});
#endregion

#region Vehicles
// TODO: Vehicles
#endregion

#region Application
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
