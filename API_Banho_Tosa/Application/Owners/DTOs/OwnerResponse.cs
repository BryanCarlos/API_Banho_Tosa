using Newtonsoft.Json;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerResponse(
        [JsonProperty("uuid")]
        Guid Uuid,

        [JsonProperty("created_at")]
        DateTime CreatedAt,

        [JsonProperty("name")]
        string Name,

        [JsonProperty("phone")]
        string? Phone = null,

        [JsonProperty("address")]
        string? Address = null
    );
}
