using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace API_Banho_Tosa.Domain.Entities
{
    [Table("roles")]
    public class Role
    {
        [Column("role_id")]
        public int Id { get; private set; }

        [Column("role_description")]
        [MaxLength(100)]
        public string Description { get; private set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        public ICollection<User> Users { get; } = new List<User>();

        private Role() { }

        private Role(int id, string description, DateTime createdAt)
        {
            this.Id = id;
            this.Description = description;

            this.CreatedAt = createdAt;
            this.UpdatedAt = createdAt;
        }

        public static Role Create(int id, string description, DateTime? createdAt = null)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Role ID must be higher than 0.");
            }

            ValidateDescription(description);

            return new Role(id, description, createdAt ?? DateTime.UtcNow);
        }

        public void UpdateDescription(string description)
        {
            ValidateDescription(description);

            this.Description = description;
            MarkAsUpdated();
        }

        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("The role description cannot be null or empty.");
            }

            if (description.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(Description), "The role description cannot exceed 100 characters.");
            }
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}
