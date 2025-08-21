using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IOwnerRepository
    {
        Task<IEnumerable<Owner>> GetOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(int id);
        Task<Owner?> GetOwnerByUuidAsync(Guid uuid);
        Task<Owner?> GetOwnerByUuidIgnoringFiltersAsync(Guid uuid);
        Task<IEnumerable<Owner>> SearchOwnersAsync(SearchOwnerRequest seachParams);
        Task<IEnumerable<Owner>> GetOwnersByNameAsync(string name);
        Task<IEnumerable<Owner>> GetOwnersByPhoneAsync(string phone);
        Task<IEnumerable<Owner>> GetArchivedOwnersAsync();
        Task InsertOwnerAsync(Owner owner);
        Task DeleteOwnerAsync(Owner owner);
        Task UpdateOwnerAsync(Owner owner);
        Task<int> SaveChangesAsync();
    }
}
