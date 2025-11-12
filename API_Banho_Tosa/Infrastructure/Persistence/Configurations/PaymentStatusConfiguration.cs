using API_Banho_Tosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class PaymentStatusConfiguration : IEntityTypeConfiguration<PaymentStatus>
    {
        public void Configure(EntityTypeBuilder<PaymentStatus> builder)
        {
            builder.ToTable("payment_status");

            builder.Property(ps => ps.Id)
                   .HasColumnName("payment_status_id")
                   .UseIdentityColumn();

            builder.HasIndex(ps => ps.Description)
                   .IsUnique();

            builder.Property(ps => ps.Description)
                   .HasColumnName("payment_status_description")
                   .HasMaxLength(100);
        }
    }
}
