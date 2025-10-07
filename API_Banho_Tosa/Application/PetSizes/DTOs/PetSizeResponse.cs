using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.PetSizes.DTOs
{
    public record PetSizeResponse
    (
        [property: JsonPropertyName("id")]
        int Id,

        [property: JsonPropertyName("description")]
        string Description
    );
}
