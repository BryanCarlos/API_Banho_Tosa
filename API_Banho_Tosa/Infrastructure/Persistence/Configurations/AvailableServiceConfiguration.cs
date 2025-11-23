using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class AvailableServiceConfiguration : IEntityTypeConfiguration<AvailableService>
    {
        public void Configure(EntityTypeBuilder<AvailableService> builder)
        {
            builder.ToTable("available_services");

            builder.HasQueryFilter(avs => avs.DeletedAt == null);

            builder.Property(avs => avs.Id)
                   .HasColumnName("available_service_id")
                   .UseIdentityColumn();

            builder.Property(avs => avs.Uuid)
                   .HasColumnName("available_service_uuid")
                   .IsRequired();

            builder.Property(avs => avs.Description)
                   .HasColumnName("description")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(avs => avs.ServiceDurationMinutes)
                   .HasColumnName("available_service_duration_minutes")
                   .IsRequired(false);

            builder.Property(avs => avs.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(avs => avs.UpdatedAt)
                   .HasColumnName("updated_at")
                   .IsRequired();

            builder.Property(avs => avs.DeletedAt)
                   .HasColumnName("deleted_at")
                   .IsRequired(false);
        }
    }
}
