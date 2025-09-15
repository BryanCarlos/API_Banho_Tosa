using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Owners.Services
{
    public interface IOwnerService
    {
        Task<OwnerResponse> CreateOwnerAsync(OwnerRequest dto);
        Task<IEnumerable<OwnerResponse>> SearchOwnersAsync(SearchOwnerRequest searchParams);
        Task<IEnumerable<OwnerResponseFullInfo>> GetArchivedOwners(string? requestingIpAddress);
        Task<IEnumerable<OwnerResponseFullInfo>> GetOwnersFullInfo(string? requestingIpAddress);
        Task DeleteOwnerByUuid(Guid uuid, string? requestingIpAddress);
        Task<OwnerResponse?> GetOwnerByUuid(Guid uuid);
        Task<OwnerResponse> UpdateOwner(Guid uuid, OwnerRequest dto);
        Task<OwnerResponse> ReactivateOwner(Guid uuid, string? requestingIpAddress);
    }
}
