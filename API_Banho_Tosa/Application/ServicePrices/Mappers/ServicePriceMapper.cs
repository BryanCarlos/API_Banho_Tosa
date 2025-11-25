using API_Banho_Tosa.Application.AvailableServices.Mappers;
using API_Banho_Tosa.Application.PetSizes.Mappers;
using API_Banho_Tosa.Application.ServicePrices.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.ServicePrices.Mappers
{
    public static class ServicePriceMapper
    {
        public static ServicePriceResponse ToResponse(this ServicePrice servicePrice)
        {
            return new ServicePriceResponse(
                servicePrice.Price,
                servicePrice.PetSize.ToResponse()
            );
        }
    }
}
