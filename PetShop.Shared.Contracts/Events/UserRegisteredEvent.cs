using System.Text.Json.Serialization;

namespace PetShop.Shared.Contracts.Events
{
    public record UserRegisteredEvent
    (
        [property: JsonPropertyName("email")]
        string Email,

        [property: JsonPropertyName("token")]
        string Token,

        [property: JsonPropertyName("expires_at")]
        DateTime ExpiresAt
    );
}
