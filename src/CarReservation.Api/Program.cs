using CarReservation.Api.Infraestructure;
using CarReservation.Api.Interfaces;
using CarReservation.Api.Interfaces.Infraestructure;
using CarReservation.Api.Interfaces.Repositories;
using CarReservation.Api.Models.Mapper;
using CarReservation.Api.Models.Validations;
using CarReservation.Api.Repositories;
using CarReservation.Api.Services;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Car Booking App", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddValidatorsFromAssemblyContaining<CarRequestValidator>();
builder.Services.AddSingleton<ICurrentDate, CurrentDate>();
builder.Services.AddSingleton<ICarRepository, CarRepository>();
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<ICarService, CarService>();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<CarProfile>();
    config.AddProfile<ReservationProfile>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => 
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Booking App V1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseAuthorization();

app.MapControllers();

app.Run();