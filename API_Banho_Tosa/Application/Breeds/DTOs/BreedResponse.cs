using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Breeds.DTOs
{
    public record BreedResponse(
        [property : JsonPropertyName("id")]
        int Id,

        [property : JsonPropertyName("name")]
        string Name,

        [property : JsonPropertyName("animal_type")]
        AnimalTypeResponse AnimalType
    );
}
