using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Auth.DTOs
{
    public record RefreshTokenRequest
    (
        [property: JsonPropertyName("refresh_token")]
        string RefreshToken
    );
}
