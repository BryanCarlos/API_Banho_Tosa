using API_Banho_Tosa.Domain.Constants;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.ValueObjects;
using Datadog.Trace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace API_Banho_Tosa.Infrastructure.Persistence
{
    public class BanhoTosaContext(DbContextOptions<BanhoTosaContext> options) : DbContext(options)
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PetSize> PetSizes { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public DbSet<ServiceStatus> ServiceStatuses { get; set; }
        public DbSet<AvailableService> AvailableServices { get; set; }
        public DbSet<ServicePrice> ServicePrices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("unaccent");

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BanhoTosaContext).Assembly);
        }
    }
}
