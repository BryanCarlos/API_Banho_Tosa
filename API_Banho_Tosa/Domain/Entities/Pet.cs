using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql.Replication;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Banho_Tosa.Domain.Entities
{
    [Table("pets")]
    public class Pet
    {
        [Column("pet_id")]
        public Guid Id { get; private set; }

        [Column("pet_name")]
        public string Name { get; private set; } = string.Empty;

        [Column("breed_id")]
        public int BreedId { get; private set; }

        [Column("pet_size_id")]
        public int PetSizeId { get; private set; }

        [Column("pet_birthdate")]
        public DateTime? BirthDate { get; private set; }

        [Column("pet_latest_visit")]
        public DateTime? LatestVisit { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; private set; }

        [NotMapped]
        public int? Age => CalculateAge();

        public Breed Breed { get; private set; } = null!;
        public PetSize PetSize { get; private set; } = null!;
        public ICollection<Owner> Owners { get; } = new List<Owner>();

        public ICollection<Service> Services = new List<Service>();

        private Pet() { }

        private Pet(string name, int breedId, int petSizeId, DateTime? birthDate)
        {
            ValidateName(name);
            if (breedId <= 0) throw new ArgumentException("Breed ID is required.");
            if (petSizeId <= 0) throw new ArgumentException("Pet Size ID is required.");

            this.Id = Guid.CreateVersion7();
            this.Name = name;
            this.BreedId = breedId;
            this.PetSizeId = petSizeId;
            this.BirthDate = birthDate;

            var now = DateTime.UtcNow;
            this.CreatedAt = now;
            this.DeletedAt = now;
        }

        public static Pet Create(string name, int breedId, int petSizeId, DateTime? birthDate)
        {
            return new Pet(name, breedId, petSizeId, birthDate);
        }

        private int? CalculateAge()
        {
            if (this.BirthDate == null)
            {
                return null;
            }
            
            var today = DateTime.UtcNow;
            var birthDate = this.BirthDate.Value;
            int age = today.Year - birthDate.Year;

            if (today < birthDate.AddYears(age))
            {
                age--;
            }

            return age;
        }

        public void Delete()
        {
            this.DeletedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The pet name cannot be null or empty.");
            }
        }
    }
}
