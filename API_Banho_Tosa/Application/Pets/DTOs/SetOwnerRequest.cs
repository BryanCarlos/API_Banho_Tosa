using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Pets.DTOs
{
    public record SetOwnerRequest
    (
        [property: JsonPropertyName("owner_id")]
        Guid OwnerId
    );
}
