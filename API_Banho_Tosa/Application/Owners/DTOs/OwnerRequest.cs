using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public class OwnerRequest
    {
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(255)]
        [JsonProperty("name")]
        public required string Name { get; set; }

        [MinLength(10)]
        [MaxLength(25)]
        [JsonProperty("phone")]
        public string? Phone { get; set; }

        [MaxLength(500)]
        [JsonProperty("address")]
        public string? Address { get; set; }
    }
}
