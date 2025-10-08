using API_Banho_Tosa.Application.PetSizes.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.PetSizes.Mappers
{
    public static class PetSizeMapper
    {
        public static PetSizeResponse ToResponse(this PetSize petSize)
        {
            return new PetSizeResponse
            (
                Id: petSize.Id,
                Description: petSize.Description
            );
        }

        public static IEnumerable<PetSizeResponse> ToEnumerableResponse(this IEnumerable<PetSize> petSizes)
        {
            return petSizes.Select(ToResponse);
        }
    }
}
