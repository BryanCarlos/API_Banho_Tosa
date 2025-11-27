using API_Banho_Tosa.Application.Services.DTOs;

namespace API_Banho_Tosa.Application.Services.Services
{
    public interface IServiceService
    {
        Task<ServiceResponse> CreateServiceAsync(CreateServiceRequest request);

        Task<ServiceResponse> GetServiceByUuidAsync(Guid id);
        Task<IEnumerable<ServiceResponse>> SearchServicesAsync(ServiceFilterQuery filter);
        Task<IEnumerable<ServiceResponse>> SearchDeletedServicesAsync(ServiceFilterQuery filter);

        Task<ServiceResponse> UpdateServiceAsync(Guid id, UpdateServiceRequest request);
        Task<ServiceResponse> UpdateServiceStatusAsync(Guid id, int newStatusId);
        Task<ServiceResponse> UpdatePaymentStatusAsync(Guid id, int newPaymentStatusId);

        Task DeleteServiceAsync(Guid id);
        Task ReactivateServiceAsync(Guid id);
    }
}
