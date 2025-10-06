using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Application.PetSizes.DTOs;

namespace API_Banho_Tosa.Application.PetSizes.Services
{
    public interface IPetSizeService
    {
        Task<PetSizeResponse> CreatePetSizeAsync(PetSizeRequest request);
        Task DeletePetSizeByIdAsync(int id);
        Task<PetSizeResponse?> GetPetSizeByIdAsync(int id);
        Task<PetSizeResponse> UpdatePetSizeAsync(int id, PetSizeRequest request);
        Task<IEnumerable<PetSizeResponse>> SearchPetSizesAsync(SearchPetSizeRequest searchParams);
    }
}
