using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Auth.DTOs
{
    public record UserLoginRequest
    (
      [property: JsonPropertyName("email")]
      string Email,

      [property: JsonPropertyName("password")]
      string Password
    );
}
