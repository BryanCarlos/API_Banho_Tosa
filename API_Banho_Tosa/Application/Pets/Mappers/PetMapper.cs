using API_Banho_Tosa.Application.Breeds.Mappers;
using API_Banho_Tosa.Application.Owners.Mappers;
using API_Banho_Tosa.Application.Pets.DTOs;
using API_Banho_Tosa.Application.PetSizes.Mappers;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Pets.Mappers
{
    public static class PetMapper
    {
        public static PetResponse ToResponse(this Pet pet)
        {
            return new PetResponse(
                pet.Id,
                pet.Name,
                pet.LatestVisit,
                pet.BirthDate,
                pet.Breed.ToResponseDto(),
                pet.PetSize.ToResponse(),
                pet.Owners.Select(o => o.MapToResponse())
            );
        }
    }
}
