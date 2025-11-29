using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.HasQueryFilter(p => p.DeletedAt == null);

            builder.HasMany(p => p.Owners)
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
        }
    }
}
