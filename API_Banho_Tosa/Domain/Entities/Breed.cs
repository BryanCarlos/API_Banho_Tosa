using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Banho_Tosa.Domain.Entities
{
    [Table("breeds")]
    public class Breed
    {
        [Column("breed_id")]
        public int Id { get; private set; }

        [Column("breed_name")]
        [MaxLength(100)]
        public string Name { get; private set; } = string.Empty;

        [Column("animal_type_id")]
        public int AnimalTypeId { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        public AnimalType AnimalType { get; private set; } = null!;

        private Breed() { }

        private Breed(string name, int animalTypeId)
        {
            ValidateName(name);

            if (animalTypeId <= 0)
            {
                throw new ArgumentException("Animal type ID is required.");
            }

            this.Name = name;
            this.AnimalTypeId = animalTypeId;

            var now = DateTime.UtcNow;
            this.CreatedAt = now;
            this.UpdatedAt = now;
        }

        public static Breed Create(string name, int animalTypeId)
        {
            return new Breed(name, animalTypeId);
        }

        public void UpdateName(string name)
        {
            ValidateName(name);

            if (this.Name == name) return;

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
                throw new ArgumentException("The breed name cannot be null or empty.");

            if (name.Length > 100)
                throw new ArgumentOutOfRangeException(nameof(Name), "The breed name cannot exceed 100 characters.");
        }
    }
}
