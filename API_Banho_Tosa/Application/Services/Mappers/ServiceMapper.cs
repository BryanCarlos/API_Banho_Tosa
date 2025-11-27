using API_Banho_Tosa.Application.PaymentStatuses.Mappers;
using API_Banho_Tosa.Application.Pets.Mappers;
using API_Banho_Tosa.Application.Services.DTOs;
using API_Banho_Tosa.Application.ServiceStatuses.Mappers;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Services.Mappers
{
    public static class ServiceMapper
    {
        public static ServiceResponse ToResponse(this Service service)
        {
            return new ServiceResponse(
                service.Uuid,
                service.ServiceDate,
                service.ServiceStatus.ToResponse(),
                service.PaymentStatus.ToResponse(),
                service.PaymentDate,
                service.PaymentDueTime,
                service.Subtotal,
                service.DiscountValue,
                service.AdditionalCharges,
                service.FinalTotal,
                service.Pet.ToResponse(),
                service.ServiceItems.Select(si => si.ToResponse())
            );
        }
    }
}
