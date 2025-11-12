using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("services");

            builder.Property(s => s.Id)
                   .HasColumnName("service_id")
                   .UseIdentityColumn();

            builder.Property(s => s.ServiceDate)
                   .HasColumnName("service_date")
                   .IsRequired();

            builder.Property(s => s.PetId)
                   .HasColumnName("pet_id")
                   .IsRequired();

            builder.Property(s => s.ServiceStatusId)
                   .HasColumnName("service_status_id")
                   .IsRequired();

            builder.Property(s => s.PaymentStatusId)
                   .HasColumnName("payment_status_id")
                   .IsRequired();

            builder.Property(s => s.PaymentDate)
                   .HasColumnName("payment_date")
                   .IsRequired(false);

            builder.Property(s => s.PaymentDueTime)
                   .HasColumnName("payment_due_date")
                   .IsRequired();

            builder.Property(s => s.Subtotal)
                   .HasColumnName("subtotal")
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(s => s.AdditionalCharges)
                   .HasColumnName("additional_charges")
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(s => s.DiscountValue)
                   .HasColumnName("discount_value")
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(s => s.FinalTotal)
                   .HasColumnName("final_total")
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(s => s.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(s => s.UpdatedAt)
                   .HasColumnName("updated_at")
                   .IsRequired();

            builder.Property(s => s.DeletedAt)
                   .HasColumnName("deleted_at")
                   .IsRequired(false);

            builder.HasOne(s => s.Pet)
                   .WithMany(p => p.Services)
                   .HasForeignKey(s => s.PetId)
                   .IsRequired();
        }
    }
}
