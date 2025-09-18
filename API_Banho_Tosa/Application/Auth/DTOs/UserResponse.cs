using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Auth.DTOs
{
    public record UserResponse
    (
        [property: JsonPropertyName("uuid")]
        Guid Uuid,

        [property: JsonPropertyName("created_at")]
        DateTime CreatedAt,

        [property: JsonPropertyName("name")]
        string Name,

        [property: JsonPropertyName("email")]
        string Email
    );
}
