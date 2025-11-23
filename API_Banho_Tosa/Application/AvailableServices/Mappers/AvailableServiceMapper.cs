using API_Banho_Tosa.Application.AvailableServices.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.AvailableServices.Mappers
{
    public static class AvailableServiceMapper
    {
        public static AvailableServiceResponse ToResponse(this AvailableService availableService)
        {
            return new AvailableServiceResponse(
                availableService.Uuid,
                availableService.Description,
                availableService.ServiceDurationMinutes
            );
        }
    }
}
