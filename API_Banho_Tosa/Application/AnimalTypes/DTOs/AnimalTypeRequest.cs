using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace API_Banho_Tosa.Application.AnimalTypes.DTOs
{
    public record AnimalTypeRequest(
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(100)]
        [JsonProperty("name")]
        string Name
    );
}
