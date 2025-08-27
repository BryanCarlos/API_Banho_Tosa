using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace API_Banho_Tosa.Infrastructure.Persistence
{
    public class BanhoTosaContext(DbContextOptions<BanhoTosaContext> options) : DbContext(options)
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Owner builder

            modelBuilder.Entity<Owner>(owner =>
            {
                owner.Property(o => o.Phone)
                     .HasConversion(
                         phoneNumber => phoneNumber.Value,
                         valueFromDb => PhoneNumber.Create(valueFromDb)
                     )
                     .HasColumnName("owner_phone");
            });

            modelBuilder
                .Entity<Owner>()
                .Property(o => o.Id)
                .UseIdentityColumn();

            modelBuilder
                .Entity<Owner>()
                .Property(o => o.Uuid)
                .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

            modelBuilder
                .Entity<Owner>()
                .HasQueryFilter(o => o.DeletedAt == null);

            #endregion

            #region AnimalType builder

            modelBuilder
                .Entity<AnimalType>()
                .Property(o => o.Id)
                .UseIdentityColumn();

            #endregion
        }
    }
}
