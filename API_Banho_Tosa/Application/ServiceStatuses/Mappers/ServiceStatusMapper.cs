using API_Banho_Tosa.Application.ServiceStatuses.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.ServiceStatuses.Mappers
{
    public static class ServiceStatusMapper
    {
        public static ServiceStatusResponse ToResponse(this ServiceStatus serviceStatus)
        {
            return new ServiceStatusResponse(
                serviceStatus.Id,
                serviceStatus.Description
            );
        }
    }
}
