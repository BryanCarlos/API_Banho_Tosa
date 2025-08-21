using Newtonsoft.Json;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public class OwnerResponse
    {
        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("phone")]
        public string? Phone { get; set; }

        [JsonProperty("address")]
        public string? Address { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
