using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Auth.DTOs
{
    public record UserCreateRequest
    (
      [property: JsonPropertyName("email")]
      string Email,

      [property: JsonPropertyName("name")]
      string Name,

      [property: JsonPropertyName("password")]
      string Password
    );
}
