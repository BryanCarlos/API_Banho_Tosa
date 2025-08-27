using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerRequest(
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(255)]
        [JsonProperty("name")]
        string Name,

        [MinLength(10)]
        [MaxLength(25)]
        [JsonProperty("phone")]
        string? Phone = null,

        [MaxLength(500)]
        [JsonProperty("address")]
        string? Address = null
    );
}
