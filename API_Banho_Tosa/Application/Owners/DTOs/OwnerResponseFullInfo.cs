using Newtonsoft.Json;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public class OwnerResponseFullInfo : OwnerResponse
    {
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
