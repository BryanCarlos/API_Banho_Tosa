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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BanhoTosaContext).Assembly);

            #region Owner builder

            modelBuilder.Entity<Owner>(owner =>
            {
                owner.Property(o => o.Id)
                     .UseIdentityColumn();

                owner.HasQueryFilter(o => o.DeletedAt == null);

                owner.Property(o => o.Phone)
                     .HasConversion(
                         phoneNumber => phoneNumber!.Value,
                         valueFromDb => PhoneNumber.Create(valueFromDb)
                     )
                     .Metadata.SetValueComparer(new ValueComparer<PhoneNumber>(
                         (p1, p2) => (p1 == null && p2 == null) || (p1 != null && p2 != null && p1.Value == p2.Value),
                         p => p != null ? p.Value.GetHashCode() : 0
                     ));
                     
                
                owner.Property(o => o.Phone).HasColumnName("owner_phone");
            });

            #endregion

            #region AnimalType builder

            modelBuilder
                .Entity<AnimalType>()
                .Property(o => o.Id)
                .UseIdentityColumn();

            #endregion

            #region Breed builder

            modelBuilder
                .Entity<Breed>()
                .Property(b => b.Id)
                .UseIdentityColumn();

            #endregion

            #region User/Roles builder

            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder
                .Entity<Role>(role =>
                {
                    role.Property(r => r.Id)
                        .UseIdentityColumn();

                    role.HasData(
                        Role.Create(id: 1, description: AppRoles.Admin, createdAt: seedDate),
                        Role.Create(id: 2, description: AppRoles.User, createdAt: seedDate)
                    );
                });


            modelBuilder
                .Entity<User>(user =>
                {
                    user.Property(u => u.Id)
                        .HasColumnName("user_uuid");

                    user.HasQueryFilter(u => u.DeletedAt == null);

                    user.Property(u => u.Email)
                        .HasConversion(
                            email => email.Value,
                            valueFromDb => Email.Create(valueFromDb)
                        )
                        .Metadata.SetValueComparer(new ValueComparer<Email>(
                            (e1, e2) => (e1 == null && e2 == null) || (e1 != null && e2 != null && e1.Value == e2.Value),
                            e => e != null ? e.Value.GetHashCode() : 0)
                        );
                        
                    user.Property(u => u.Email)
                        .HasColumnName("user_email");

                    user.HasIndex(u => u.RefreshToken).IsUnique();
                });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "users_roles",
                    j => j.HasOne<Role>()
                          .WithMany()
                          .HasForeignKey("role_id"),
                    l => l.HasOne<User>()
                          .WithMany()
                          .HasForeignKey("user_id")
            );

            #endregion

            #region PetSize builder

            modelBuilder
                .Entity<PetSize>()
                .Property(p => p.Id)
                .UseIdentityColumn();

            #endregion

            #region Pet builder

            modelBuilder.Entity<Pet>(pet =>
            {
                pet.HasQueryFilter(p => p.DeletedAt == null);

                pet.HasMany(p => p.Owners)
                    .WithMany(o => o.Pets)
                    .UsingEntity<Dictionary<string, object>>(
                        "pets_owners",
                        j => j.HasOne<Owner>()
                                .WithMany()
                                .HasForeignKey("owner_id"),
                        l => l.HasOne<Pet>()
                                .WithMany()
                                .HasForeignKey("pet_id")
                    );
            });

            #endregion
        }
    }
}
