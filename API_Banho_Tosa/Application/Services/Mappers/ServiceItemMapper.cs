using API_Banho_Tosa.Application.Services.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Services.Mappers
{
    public static class ServiceItemMapper
    {
        public static ServiceItemResponse ToResponse(this ServiceItem item)
        {
            return new ServiceItemResponse(
                item.Id,
                item.AvailableService.Description,
                item.PriceAtTheTime,
                item.Quantity,
                item.Quantity * item.PriceAtTheTime
            );
        }
    }
}
