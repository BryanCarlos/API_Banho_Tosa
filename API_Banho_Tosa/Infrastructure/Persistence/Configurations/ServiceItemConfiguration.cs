using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class ServiceItemConfiguration : IEntityTypeConfiguration<ServiceItem>
    {
        public void Configure(EntityTypeBuilder<ServiceItem> builder)
        {
            builder.ToTable("service_items");

            builder.Property(si => si.Id)
                   .HasColumnName("service_item_id")
                   .UseIdentityColumn();

            builder.Property(si => si.ServiceId)
                   .HasColumnName("service_id")
                   .IsRequired();

            builder.Property(si => si.AvailableServiceId)
                   .HasColumnName("available_service")
                   .IsRequired();

            builder.Property(si => si.PriceAtTheTime)
                   .HasColumnName("price_at_the_time")
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(si => si.Quantity)
                   .HasColumnName("quantity")
                   .IsRequired();

            builder.Property(si => si.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(si => si.UpdatedAt)
                   .HasColumnName("updated_at")
                   .IsRequired();

            builder.HasOne(si => si.Service)
                   .WithMany(s => s.ServiceItems)
                   .HasForeignKey(si => si.ServiceId)
                   .IsRequired();

            builder.HasOne(si => si.AvailableService)
                   .WithMany(avs => avs.ServiceItems)
                   .HasForeignKey(si => si.AvailableServiceId)
                   .IsRequired();

            builder.HasQueryFilter(
                si => si.Service.DeletedAt == null &&
                si.Service.Pet.DeletedAt == null
            );
        }
    }
}
