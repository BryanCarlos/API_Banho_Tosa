using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IServicePriceRepository
    {
        Task<bool> ServicePriceExistAsync(int serviceId, int petSizeId);
        Task<ServicePrice?> GetServicePriceByCompositeKeyAsync(int serviceId, int petSizeId);
        Task<IEnumerable<ServicePrice>> GetAllByServiceIdAsync(int serviceId);
        Task<IEnumerable<ServicePrice>> GetAllByServiceUuidAsync(Guid serviceId);
        void AddServicePrice(ServicePrice servicePrice);
        void DeleteServicePrice(ServicePrice servicePrice);
        Task<int> SaveChangesAsync();
    }
}
