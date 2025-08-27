using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Owners.Services
{
    public interface IOwnerService
    {
        Task<OwnerResponse> CreateOwnerAsync(OwnerRequest dto);
        Task<IEnumerable<OwnerResponse>> SearchOwnersAsync(SearchOwnerRequest searchParams);
        Task<IEnumerable<OwnerResponseFullInfo>> GetArchivedOwners();
        Task<IEnumerable<OwnerResponseFullInfo>> GetOwnersFullInfo();
        Task DeleteOwnerByUuid(Guid uuid);
        Task<OwnerResponse?> GetOwnerByUuid(Guid uuid);
        Task<OwnerResponse> UpdateOwner(Guid uuid, OwnerRequest dto);
        Task<OwnerResponse> ReactivateOwner(Guid uuid);
    }
}
