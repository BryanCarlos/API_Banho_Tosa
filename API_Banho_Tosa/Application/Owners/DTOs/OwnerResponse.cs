
using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerResponse(
        [property:JsonPropertyName("uuid")]
        Guid Uuid,

        [property:JsonPropertyName("created_at")]
        DateTime CreatedAt,

        [property:JsonPropertyName("name")]
        string Name,

        [property:JsonPropertyName("phone")]
        string? Phone = null,

        [property:JsonPropertyName("address")]
        string? Address = null
    );
}
