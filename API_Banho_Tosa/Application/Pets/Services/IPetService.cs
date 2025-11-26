using API_Banho_Tosa.Application.Pets.DTOs;

namespace API_Banho_Tosa.Application.Pets.Services
{
    public interface IPetService
    {
        Task<PetResponse> CreatePetAsync(CreatePetRequest request);
        Task<PetResponse> UpdatePetAsync(Guid id, UpdatePetRequest request);
        Task<PetResponse> GetPetByIdAsync(Guid id);
        Task<PetResponse> SetNewOwnerAsync(Guid id, SetOwnerRequest request);
        Task RemoveOwnerAsync(Guid petId, Guid ownerId);
        Task DeletePetAsync(Guid id);
        Task ReactivatePetAsync(Guid id);

        Task<IEnumerable<PetResponse>> SearchPetsAsync(PetFilterQuery filter);
        Task<IEnumerable<PetResponse>> SearchDeletedPetsAsync(PetFilterQuery filter);
    }
}
