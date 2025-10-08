using System.ComponentModel.DataAnnotations.Schema;

namespace API_Banho_Tosa.Domain.Entities
{
    [Table("pet_sizes")]
    public class PetSize
    {
        [Column("pet_size_id")]
        public int Id { get; private set; }

        [Column("pet_size_description")]
        public string Description { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        public HashSet<Pet> Pets { get; } = new HashSet<Pet>();

        private PetSize() 
        {
            this.Description = string.Empty;
        }

        private PetSize(string description)
        {
            this.Description = description;

            var now = DateTime.UtcNow;
            this.CreatedAt = now;
            this.UpdatedAt = now;
        }

        public static PetSize Create(string description)
        {
            ValidateDescription(description);

            return new PetSize(description);
        }

        public void UpdateDescription(string description)
        {
            ValidateDescription(description);

            this.Description = description;
            MarkAsUpdated();
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("The pet size description cannot be null or empty.");
            }
        }
    }
}
