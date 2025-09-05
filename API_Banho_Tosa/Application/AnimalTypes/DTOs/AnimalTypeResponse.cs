using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.AnimalTypes.DTOs
{
    public record AnimalTypeResponse(
        [property : JsonPropertyName("id")]
        int Id,

        [property : JsonPropertyName("name")]
        string Name
    );
}
