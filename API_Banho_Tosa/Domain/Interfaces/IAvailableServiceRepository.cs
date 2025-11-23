using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IAvailableServiceRepository
    {
        Task<IEnumerable<AvailableService>> SearchAvailableServicesAsync(string? description);
        Task<IEnumerable<AvailableService>> SearchDeletedAvailableServicesAsync(string? description);
        Task<AvailableService?> GetAvailableServiceByIdAsync(int id);
        Task<AvailableService?> GetAvailableServiceByUuidAsync(Guid uuid);
        Task<AvailableService?> GetDeletedAvailableServiceByUuidAsync(Guid uuid);

        Task<bool> AvailableServiceExistAsync(string description);

        void AddAvailableService(AvailableService availableService);
        Task<int> SaveChangesAsync();
    }
}
