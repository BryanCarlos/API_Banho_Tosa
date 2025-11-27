using API_Banho_Tosa.Domain.Common.Extensions;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class ServiceStatusConfiguration : IEntityTypeConfiguration<ServiceStatus>
    {
        public void Configure(EntityTypeBuilder<ServiceStatus> builder)
        {
            builder.ToTable("service_status");

            builder.Property(ss => ss.Id)
                   .HasColumnName("service_status_id")
                   .UseIdentityColumn();

            builder.HasIndex(ss => ss.Description)
                   .IsUnique();

            builder.Property(ss => ss.Description)
                   .HasColumnName("service_status_description")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.HasMany(ss => ss.Services)
                   .WithOne(s => s.ServiceStatus)
                   .HasForeignKey(s => s.ServiceStatusId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                ServiceStatus.Create(ServiceStatusEnum.Agendado.GetDescription(), (int)ServiceStatusEnum.Agendado),
                ServiceStatus.Create(ServiceStatusEnum.Concluido.GetDescription(), (int)ServiceStatusEnum.Concluido),
                ServiceStatus.Create(ServiceStatusEnum.Cancelado.GetDescription(), (int)ServiceStatusEnum.Cancelado)
            );
        }
    }
}
