using API_Banho_Tosa.Application.AvailableServices.DTOs;

namespace API_Banho_Tosa.Application.AvailableServices.Services
{
    public interface IAvailableServiceService
    {
        Task<IEnumerable<AvailableServiceResponse>> SearchAvailableServicesAsync(AvailableServiceFilterQuery filter);
        Task<IEnumerable<AvailableServiceResponse>> SearchDeletedAvailableServicesAsync(AvailableServiceFilterQuery filter);

        Task<AvailableServiceResponse> CreateAvailableServiceAsync(CreateAvailableServiceRequest request);
        Task<AvailableServiceResponse> UpdateAvailableServiceAsync(Guid id, UpdateAvailableServiceRequest request);
        Task<AvailableServiceResponse> GetAvailableServiceByUuidAsync(Guid id);
        Task DeleteAvailableServiceAsync(Guid id);
        Task ReactivateAvailableServiceAsync(Guid id);
    }
}
