using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.AnimalTypes.Mappers
{
    public static class AnimalTypeMapper
    {
        public static IEnumerable<AnimalTypeResponse> ToEnumerableResponseDto(this IEnumerable<AnimalType> animalTypes)
        {
            return animalTypes.Select(at => at.ToResponseDto());
        }

        public static AnimalTypeResponse ToResponseDto(this AnimalType animalType)
        {
            return new AnimalTypeResponse(animalType.Id, animalType.Name);
        }
    }
}
