using API_Banho_Tosa.Application.AnimalTypes.Mappers;
using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Breeds.Mappers
{
    public static class BreedMapper
    {
        public static IReadOnlyCollection<BreedResponse> ToColletionDto(this IReadOnlyCollection<Breed> breeds)
        {
            return breeds.Select(b => b.ToResponseDto()).ToList();
        }

        public static BreedResponse ToResponseDto(this Breed breed)
        {
            return new BreedResponse(breed.Id, breed.Name, breed.AnimalType.ToResponseDto());
        }
    }
}
