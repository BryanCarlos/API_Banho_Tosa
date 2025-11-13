using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class ServicePriceConfiguration : IEntityTypeConfiguration<ServicePrice>
    {
        public void Configure(EntityTypeBuilder<ServicePrice> builder)
        {
            builder.ToTable("service_prices");

            builder.HasKey(sp => new { sp.AvailableServiceId, sp.PetSizeId });

            builder.HasOne(sp => sp.AvailableService)
                   .WithMany(avs => avs.ServicePrices)
                   .HasForeignKey(sp => sp.AvailableServiceId);

            builder.HasOne(sp => sp.PetSize)
                   .WithMany(ps => ps.ServicePrices)
                   .HasForeignKey(sp => sp.PetSizeId);

            builder.Property(sp => sp.AvailableServiceId)
                   .HasColumnName("available_service_id");

            builder.Property(sp => sp.PetSizeId)
                   .HasColumnName("pet_size_id");

            builder.Property(sp => sp.Price)
                   .HasColumnName("service_price")
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(sp => sp.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(sp => sp.UpdatedAt)
                   .HasColumnName("updated_at")
                   .IsRequired();
        }
    }
}
