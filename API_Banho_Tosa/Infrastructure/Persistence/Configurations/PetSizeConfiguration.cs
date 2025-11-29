using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class PetSizeConfiguration : IEntityTypeConfiguration<PetSize>
    {
        public void Configure(EntityTypeBuilder<PetSize> builder)
        {
            builder.Property(p => p.Id)
                .UseIdentityColumn();
        }
    }
}
