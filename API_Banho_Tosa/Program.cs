
using API_Banho_Tosa.Application.Owners;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Persistence;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;
using API_Banho_Tosa.Middleware;
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

            builder.Services.AddControllers();
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
