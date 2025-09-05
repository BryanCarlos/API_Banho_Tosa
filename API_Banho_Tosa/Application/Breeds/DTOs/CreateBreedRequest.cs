using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Breeds.DTOs
{
    public record CreateBreedRequest
    {
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(100)]
        [property: JsonPropertyName("name")]
        public string? Name { get; init; }

        [Required(ErrorMessage = "The animal type ID is required.")]
        [property: JsonPropertyName("animal_type_id")] 
        public int? AnimalTypeId { get; init; }
    }
}
