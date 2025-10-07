using API_Banho_Tosa.API.Services;
using API_Banho_Tosa.Application.AnimalTypes.Services;
using API_Banho_Tosa.Application.Auth.Interfaces;
using API_Banho_Tosa.Application.Auth.Services;
using API_Banho_Tosa.Application.Breeds.Services;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.Owners.Services;
using API_Banho_Tosa.Application.PetSizes.Services;
using API_Banho_Tosa.Application.Users.Services;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Auth;
using API_Banho_Tosa.Infrastructure.Persistence;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;
using API_Banho_Tosa.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Datadog.Logs;
using System.Text;

namespace API_Banho_Tosa
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["AppSettings:Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
            builder.Services.AddScoped<IOwnerService, OwnerService>();

            builder.Services.AddScoped<IAnimalTypeRepository, AnimalTypeRepository>();
            builder.Services.AddScoped<IAnimalTypeService, AnimalTypeService>();

            builder.Services.AddScoped<IBreedRepository, BreedRepository>();
            builder.Services.AddScoped<IBreedService, BreedService>();

            builder.Services.AddScoped<IPetSizeRepository, PetSizeRepository>();
            builder.Services.AddScoped<IPetSizeService, PetSizeService>();

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
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "PetShop Manager API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Insert your JWT token:"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            var app = builder.Build();

            app.UseSerilogRequestLogging();
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
