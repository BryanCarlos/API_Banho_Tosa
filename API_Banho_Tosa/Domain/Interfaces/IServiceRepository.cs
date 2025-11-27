using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IServiceRepository
    {
        void AddService(Service service);

        Task<Service?> GetServiceByUuidAsync(Guid id);
        Task<Service?> GetDeletedServiceByUuidAsync(Guid id);

        Task<IEnumerable<Service>> SearchServicesAsync(
            DateTime? startDate,
            DateTime? endDate,
            int? statusId,
            int? paymentStatusId,
            Guid? petId,
            Guid? ownerId
        );

        Task<IEnumerable<Service>> SearchDeletedServicesAsync(
            DateTime? startDate,
            DateTime? endDate,
            int? statusId,
            int? paymentStatusId,
            Guid? petId,
            Guid? ownerId
        );

        Task<int> SaveChangesAsync();
        void DeleteServiceItems(ICollection<ServiceItem> serviceItems);
    }
}
