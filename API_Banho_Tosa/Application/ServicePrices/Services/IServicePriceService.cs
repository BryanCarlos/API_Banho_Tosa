using API_Banho_Tosa.Application.ServicePrices.DTOs;

namespace API_Banho_Tosa.Application.ServicePrices.Services
{
    public interface IServicePriceService
    {
        Task<ServicePriceResponse> CreateServicePriceAsync(CreateServicePriceRequest request);
        Task<ServicePriceResponse> UpdateServicePriceAsync(UpdateServicePriceRequest request);
        Task<IEnumerable<ServicePriceResponse>> GetServicePricesByServiceAsync(Guid serviceId);
        Task DeleteServicePriceAsync(int serviceId, int petSizeId);
        Task DeleteServicePriceAsync(Guid serviceId, int petSizeId);
        Task<ServicePriceResponse> AddServicePriceAsync(Guid id, AddServicePriceRequest request);
        Task<ServicePriceResponse> UpdateServicePriceAsync(Guid id, int petSizeId, UpdatePriceRequest request);
    }
}
