using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Auth.DTOs
{
    public record TokenResponse
    (
        [property: JsonPropertyName("access_token")]
        string AccessToken,

        [property: JsonPropertyName("refresh_token")]
        string RefreshToken
    );
}
