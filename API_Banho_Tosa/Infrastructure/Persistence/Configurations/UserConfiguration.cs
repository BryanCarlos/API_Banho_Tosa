using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
                        .HasColumnName("user_uuid");

            builder.HasQueryFilter(u => u.DeletedAt == null);

            builder.Property(u => u.Email)
                .HasConversion(
                    email => email.Value,
                    valueFromDb => Email.Create(valueFromDb)
                )
                .Metadata.SetValueComparer(new ValueComparer<Email>(
                    (e1, e2) => (e1 == null && e2 == null) || (e1 != null && e2 != null && e1.Value == e2.Value),
                    e => e != null ? e.Value.GetHashCode() : 0)
                );

            builder.Property(u => u.Email)
                .HasColumnName("user_email");

            builder.HasIndex(u => u.RefreshToken).IsUnique();

            builder.HasMany(u => u.Roles)
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
        }
    }
}
