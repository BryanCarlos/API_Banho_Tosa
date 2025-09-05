using API_Banho_Tosa.Application.AnimalTypes.Services;
using API_Banho_Tosa.Application.Breeds.Services;
using API_Banho_Tosa.Application.Owners.Services;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Persistence;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;
using API_Banho_Tosa.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<BanhoTosaContext>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("BanhoTosaContext")));

            builder.Services.AddScoped<BanhoTosaContext>();
            
            builder.Services.AddTransient<IOwnerRepository, OwnerRepository>();
            builder.Services.AddTransient<IOwnerService, OwnerService>();

            builder.Services.AddTransient<IAnimalTypeRepository, AnimalTypeRepository>();
            builder.Services.AddTransient<IAnimalTypeService, AnimalTypeService>();

            builder.Services.AddTransient<IBreedRepository, BreedRepository>();
            builder.Services.AddTransient<IBreedService, BreedService>();

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var firstErrorMessage = context.ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(v => v.ErrorMessage)
                            .FirstOrDefault();

                        var errorResponse = new { error = firstErrorMessage };

                        return new BadRequestObjectResult(errorResponse);
                    };
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionsHandlerMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
