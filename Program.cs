using CarManager.Domain.DTOs;
using CarManager.Domain.Entities;
using CarManager.Domain.Interfaces;
using CarManager.Domain.ModelViews;
using CarManager.Domain.Services;
using CarManager.Infraestructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

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
// app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
app.MapGet("/", () =>
{
    Console.WriteLine("Accessing API documentation...");
    return Results.Redirect("/swagger");
}).WithTags("Home");
#endregion

#region Administrators
app.MapPost("/administrators/login", (LoginDTO loginDTO, IAdministratorService service) =>
{
    if (service.Login(loginDTO) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
}).WithTags("Administrators");
#endregion

#region Vehicles
ValidationErrors ValidateVehicle(VehicleDTO vehicleDto)
{
    var validation = new ValidationErrors();

    if (string.IsNullOrEmpty(vehicleDto.Name))
        validation.Messages.Add("Name is required");
    if (string.IsNullOrEmpty(vehicleDto.Brand))
        validation.Messages.Add("Brand is required");
    if (vehicleDto.Year < 1900)
        validation.Messages.Add("Vehicle year must be greater than or equal to 1900");

    return validation;
}

bool HasValidationErrors(VehicleDTO dto, out ValidationErrors errors)
{
    errors = ValidateVehicle(dto);
    return errors.Messages.Count > 0;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDto, IVehicleService service) =>
{
    if (HasValidationErrors(vehicleDto, out var errors))
        return Results.BadRequest(errors);

    var vehicle = new Vehicle
    {
        Name = vehicleDto.Name,
        Brand = vehicleDto.Brand,
        Year = vehicleDto.Year
    };
    service.Add(vehicle);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService service) =>
{
    var vehicles = service.GetAll(page, 5, null, null);
    return  Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService service) =>
{
    var vehicle = service.GetById(id);
    if (vehicle == null) return Results.NotFound("Vehicle not found");

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id:int}", (int id, [FromBody] VehicleDTO vehicleDto, IVehicleService service) =>
{
    var vehicle = service.GetById(id);
    if (vehicle is null) return Results.NotFound("Vehicle not found");

    if (HasValidationErrors(vehicleDto, out var errors))
        return Results.BadRequest(errors);

    vehicle.Name = vehicleDto.Name;
    vehicle.Brand = vehicleDto.Brand;
    vehicle.Year = vehicleDto.Year;

    service.Update(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id:int}", (int id, IVehicleService service) =>
{
    var vehicle = service.GetById(id);
    if (vehicle == null) return Results.NotFound("Vehicle not found");

    service.Delete(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region Application
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
