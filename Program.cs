using CarManager.Domain.DTOs;
using CarManager.Domain.Entities;
using CarManager.Domain.Enums;
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
ValidationErrors ValidateAdministrator(AdministratorDTO administratorDto)
{
    var validation = new ValidationErrors();

    if (string.IsNullOrEmpty(administratorDto.Email))
        validation.Messages.Add("E-mail is required");
    if (string.IsNullOrEmpty(administratorDto.Password))
        validation.Messages.Add("Password is required");
    if (administratorDto.Role == AdmRole.None)
        validation.Messages.Add("Role is required");

    return validation;
}

bool HasAdministratorValidationErrors(AdministratorDTO dto, out ValidationErrors errors)
{
    errors = ValidateAdministrator(dto);
    return errors.Messages.Count > 0;
}

app.MapPost("/administrators/login", (LoginDTO loginDTO, IAdministratorService service) =>
{
    if (service.Login(loginDTO) != null)
        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
}).WithTags("Administrators");

app.MapPost("/administrators", ([FromBody] AdministratorDTO administratorDto, IAdministratorService service) =>
{
    if (HasAdministratorValidationErrors(administratorDto, out var errors))
        return Results.BadRequest(errors);

    var administrator = new Administrator
    {
        Email = administratorDto.Email,
        Password = administratorDto.Password,
        Role = administratorDto.Role
    };
    service.Add(administrator);

    return Results.Created($"/administrators/{administrator.Id}", administrator);
}).WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService service) =>
{
    var administrators = service.GetAll(page, 5);

    var administratorResponses = administrators
        .Select(a => new AdministratorResponse(a.Id, a.Email, a.Role))
        .ToList();

    return Results.Ok(administratorResponses);
}).WithTags("Administrators");

app.MapGet("/administrators/{id:int}", ([FromRoute] int id, IAdministratorService service) =>
{
    var administrator = service.GetById(id);
    if (administrator == null) return Results.NotFound("Administrator not found");

    var administratorResponse = new AdministratorResponse(
        administrator.Id,
        administrator.Email,
        administrator.Role
    );

    return Results.Ok(administratorResponse);
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

bool HasVehicleValidationErrors(VehicleDTO dto, out ValidationErrors errors)
{
    errors = ValidateVehicle(dto);
    return errors.Messages.Count > 0;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDto, IVehicleService service) =>
{
    if (HasVehicleValidationErrors(vehicleDto, out var errors))
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

    if (HasVehicleValidationErrors(vehicleDto, out var errors))
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
