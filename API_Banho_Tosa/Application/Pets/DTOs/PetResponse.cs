using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Application.PetSizes.DTOs;
using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Pets.DTOs
{
    public record PetResponse
    (
        [property: JsonPropertyName("id")]
        Guid Id,

        [property: JsonPropertyName("name")]
        string Name,

        [property: JsonPropertyName("latest_visit")]
        DateTime? LatestVisit,

        [property: JsonPropertyName("birth_date")]
        DateTime? BirthDate,

        [property: JsonPropertyName("breed")]
        BreedResponse Breed,

        [property: JsonPropertyName("pet_size")]
        PetSizeResponse PetSize,

        [property: JsonPropertyName("owners")]
        IEnumerable<OwnerResponse> Owners
    );
}
