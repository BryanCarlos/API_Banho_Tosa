using API_Banho_Tosa.Application.AnimalTypes.Services;
using API_Banho_Tosa.Application.Breeds.Services;
using API_Banho_Tosa.Application.Owners.Services;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Persistence;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;
using API_Banho_Tosa.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Datadog.Logs;

namespace API_Banho_Tosa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            SelfLog.Enable(Console.Error);

            // Add services to the container.

            var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.Console();

            var datadogEnabled = builder.Configuration.GetValue<bool>("Datadog:Enabled");

            if (datadogEnabled)
            {
                var datadogApiKey = builder.Configuration["Datadog:ApiKey"];

                if (string.IsNullOrEmpty(datadogApiKey))
                {
                    throw new InvalidOperationException("Datadog API Key not found. Configure it in User Secrets.");
                }

                var datadogConfig = new DatadogConfiguration(url: Environment.GetEnvironmentVariable("DD_URL"));

                loggerConfiguration.WriteTo.DatadogLogs(
                    apiKey: datadogApiKey,
                    service: Environment.GetEnvironmentVariable("DD_SERVICE"),
                    source: "csharp",
                    tags: new[] { $"env:{Environment.GetEnvironmentVariable("DD_ENV")}" },
                    configuration: datadogConfig
                );
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            builder.Host.UseSerilog();

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

            app.UseMiddleware<RequestLoggingMiddleware>();
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

            try
            {
                Log.Information("Starting application...");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
