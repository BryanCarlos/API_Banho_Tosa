using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API_Banho_Tosa.Infrastructure.Persistence.Configurations
{
    public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
    {
        public void Configure(EntityTypeBuilder<Owner> builder)
        {
            builder.Property(o => o.Id)
                     .UseIdentityColumn();

            builder.HasQueryFilter(o => o.DeletedAt == null);

            builder.Property(o => o.Phone)
                 .HasConversion(
                     phoneNumber => phoneNumber!.Value,
                     valueFromDb => PhoneNumber.Create(valueFromDb)
                 )
                 .Metadata.SetValueComparer(new ValueComparer<PhoneNumber>(
                     (p1, p2) => (p1 == null && p2 == null) || (p1 != null && p2 != null && p1.Value == p2.Value),
                     p => p != null ? p.Value.GetHashCode() : 0
                 ));


            builder.Property(o => o.Phone).HasColumnName("owner_phone");
        }
    }
}
