using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.PetSizes.DTOs
{
    public record PetSizeRequest
    (
        [property: JsonPropertyName("description")]
        string Description
    );
}
