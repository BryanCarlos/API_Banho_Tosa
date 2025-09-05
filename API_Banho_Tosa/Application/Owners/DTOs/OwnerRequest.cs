using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerRequest(
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(255)]
        [property: JsonPropertyName("name")]
        string Name,

        [MinLength(10)]
        [MaxLength(25)]
        [property: JsonPropertyName("phone")]
        string? Phone = null,

        [MaxLength(500)]
        [property: JsonPropertyName("address")]
        string? Address = null
    );
}
