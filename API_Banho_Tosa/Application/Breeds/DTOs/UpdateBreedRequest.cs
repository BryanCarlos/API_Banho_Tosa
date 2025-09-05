using System.ComponentModel.DataAnnotations;

namespace API_Banho_Tosa.Application.Breeds.DTOs
{
    public record UpdateBreedRequest(
        [Required]
        [MaxLength(100)]
        string Name
    );
}
