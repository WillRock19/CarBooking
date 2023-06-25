using CarReservation.Api.Interfaces;
using CarReservation.Api.Models.Mapper;
using CarReservation.Api.Models.Validations;
using CarReservation.Api.Repositories;
using CarReservation.Api.Services;
using FluentValidation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddValidatorsFromAssemblyContaining<CarRequestValidator>();
        builder.Services.AddSingleton<ICarRepository, CarRepository>();
        builder.Services.AddScoped<ICarService, CarService>();
        builder.Services.AddAutoMapper(config =>
        {
            config.AddProfile<MappingProfile>();
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}