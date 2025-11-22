using API_Banho_Tosa.Application.ServiceStatuses.DTOs;

namespace API_Banho_Tosa.Application.ServiceStatuses.Services
{
    public interface IServiceStatusService
    {
        Task<ServiceStatusResponse> CreateServiceStatusAsync(CreateServiceStatusRequest request);
        Task<ServiceStatusResponse> UpdateServiceStatusAsync(int id, UpdateServiceStatusRequest request);
        Task<IEnumerable<ServiceStatusResponse>> SearchServiceStatusesAsync(ServiceStatusFilterQuery filter);
        Task<ServiceStatusResponse> GetServiceStatusByIdAsync(int id);
        Task DeleteServiceStatusAsync(int id);
    }
}
