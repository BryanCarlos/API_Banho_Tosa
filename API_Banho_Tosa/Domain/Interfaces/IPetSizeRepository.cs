using API_Banho_Tosa.Application.PetSizes.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IPetSizeRepository
    {
        Task<PetSize?> GetPetSizeByIdAsync(int id);
        Task<PetSize?> GetPetSizeByDescriptionAsync(string description);
        Task<IEnumerable<PetSize>> SearchPetSizesAsync(SearchPetSizeRequest searchParams);
        void InsertPetSize(PetSize petSize);
        void DeletePetSize(PetSize petSize);
        Task<int> SaveChangesAsync();
    }
}
