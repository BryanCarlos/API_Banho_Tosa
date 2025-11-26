using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Pets.DTOs
{
    public record CreatePetRequest
    (
        [property: JsonPropertyName("name")]
        string Name,

        [property: JsonPropertyName("breed_id")]
        int BreedId,

        [property: JsonPropertyName("pet_size_id")]
        int PetSizeId,

        [property: JsonPropertyName("owner_id")]
        Guid OwnerId,

        [property: JsonPropertyName("birth_date")]
        DateTime? BirthDate
    );
}
