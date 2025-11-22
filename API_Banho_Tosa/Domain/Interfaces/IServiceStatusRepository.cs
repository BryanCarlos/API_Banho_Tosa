using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IServiceStatusRepository
    {
        Task<IEnumerable<ServiceStatus>> SearchServiceStatusesAsync(string? description);
        Task<ServiceStatus?> GetServiceStatusByIdAsync(int id);
        Task<bool> ServiceStatusExistAsync(string description);

        void AddServiceStatus(ServiceStatus serviceStatus);
        void DeleteServiceStatus(ServiceStatus serviceStatus);
        Task<int> SaveChangesAsync();
    }
}
