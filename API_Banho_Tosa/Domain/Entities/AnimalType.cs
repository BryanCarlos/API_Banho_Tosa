using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Banho_Tosa.Domain.Entities
{
    [Table("animal_types")]
    public class AnimalType
    {
        [Column("animal_type_id")]
        public int Id { get; private set; }

        [Column("animal_type_name")]
        [MaxLength(100)]
        public string Name { get; private set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        public HashSet<Breed> Breeds { get; } = new HashSet<Breed>();

        private AnimalType() { }

        private AnimalType(string name)
        {
            ValidateName(name);

            this.Name = name;

            var now = DateTime.UtcNow;

            this.CreatedAt = now;
            this.UpdatedAt = now;
        }

        public static AnimalType Create(string name)
        {
            return new AnimalType(name);
        }
       
        public void UpdateName(string name)
        {
            ValidateName(name);

            if (this.Name == name)
                return;

            this.Name = name;
            MarkAsUpdated();
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(Name));

            if (name.Length > 100)
                throw new ArgumentOutOfRangeException(nameof(Name));
        }
    }
}
