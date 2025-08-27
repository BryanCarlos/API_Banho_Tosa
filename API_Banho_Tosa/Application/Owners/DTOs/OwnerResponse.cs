using Newtonsoft.Json;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerResponse(
        [JsonProperty("uuid")]
        Guid Uuid,

        [JsonProperty("name")]
        string Name,

        [JsonProperty("phone")]
        string? Phone,

        [JsonProperty("address")]
        string? Address,

        [JsonProperty("created_at")]
        DateTime CreatedAt
    );
}
