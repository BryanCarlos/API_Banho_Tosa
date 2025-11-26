using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IPetRepository
    {
        void AddPet(Pet pet);
        Task<Pet?> GetPetByIdAsync(Guid id);
        Task<IEnumerable<Pet>> SearchPetsAsync(string? petName, string? ownerName, Guid? ownerId);
        Task<IEnumerable<Pet>> SearchDeletedPetsAsync(string? petName, string? ownerName, Guid? ownerId);
        Task<int> SaveChangesAsync();
        Task<Pet?> GetDeletedPetByIdAsync(Guid id);
    }
}
