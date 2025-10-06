using API_Banho_Tosa.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Banho_Tosa.Domain.Entities
{
    [Table("owners")]
    public class Owner
    {
        [Column("owner_id")]
        public int Id { get; private set; }

        [Column("owner_uuid")]
        public Guid Uuid { get; private set; }

        [Column("owner_name")]
        [MaxLength(255)]
        public string Name { get; private set; } = string.Empty;

        [Column("owner_phone")]
        public PhoneNumber? Phone { get; private set; } 

        [Column("owner_address")]
        [MaxLength(500)]
        public string? Address { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; private set; }

        public ICollection<Pet> Pets { get; } = new List<Pet>();

        // Construtor para o EF
        private Owner() { }

        // Construtor proprio para utilizar o metodo "Create"
        private Owner(string name, PhoneNumber? phone, string? address)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), "The name is required.");
            }

            this.Name = name;
            this.Phone = phone;
            this.Address = address;

            var dateNow = DateTime.UtcNow;

            this.Uuid = Guid.NewGuid();
            this.CreatedAt = dateNow;
            this.UpdatedAt = dateNow;
        }

        public static Owner Create(string name, PhoneNumber? phone = null, string? address = null)
        {
            return new Owner(name, phone, address);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) 
            {
                throw new ArgumentNullException(nameof(name), "The name must be filled in.");
            }

            if (name.Length > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "The name must've maximum 255 characters.");
            }

            this.Name = name;
            MarkAsUpdated();
        }

        public void UpdatePhone(string? phone)
        {
            this.Phone = string.IsNullOrWhiteSpace(phone) ? null : PhoneNumber.Create(phone);
            MarkAsUpdated();
        }

        public void UpdateAddress(string? address)
        {
            if (address?.Length > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(address), "The address must've maximum 500 charactes.");
            }

            this.Address = address?.Trim();
            MarkAsUpdated();
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            if (DeletedAt.HasValue)
            {
                throw new InvalidOperationException("The owner has already been deleted.");
            }

            this.DeletedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (!DeletedAt.HasValue)
            {
                throw new InvalidOperationException("The owner's already active.");
            }

            this.DeletedAt = null;
            MarkAsUpdated();
        }
    }
}
