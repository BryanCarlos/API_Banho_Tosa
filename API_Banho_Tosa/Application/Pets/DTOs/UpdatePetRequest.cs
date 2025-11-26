using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Pets.DTOs
{
    public record UpdatePetRequest
    (
        [property: JsonPropertyName("name")]
        string Name,

        [property: JsonPropertyName("breed_id")]
        int BreedId,

        [property: JsonPropertyName("pet_size_id")]
        int PetSizeId,

        [property: JsonPropertyName("latest_visit")]
        DateTime? LatestVisit,

        [property: JsonPropertyName("birth_date")]
        DateTime? BirthDate
    );
}
