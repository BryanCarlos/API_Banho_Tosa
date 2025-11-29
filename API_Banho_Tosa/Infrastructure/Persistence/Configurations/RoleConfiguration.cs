using API_Banho_Tosa.Domain.Constants;
using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Data;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.Property(r => r.Id)
                        .UseIdentityColumn();

            builder.HasData(
                Role.Create(id: 1, description: AppRoles.Admin, createdAt: seedDate),
                Role.Create(id: 2, description: AppRoles.User, createdAt: seedDate)
            );
        }
    }
}
