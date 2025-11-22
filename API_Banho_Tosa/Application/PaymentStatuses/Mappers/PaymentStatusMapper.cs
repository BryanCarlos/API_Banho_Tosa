using API_Banho_Tosa.Application.PaymentStatuses.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.PaymentStatuses.Mappers
{
    public static class PaymentStatusMapper
    {
        public static PaymentStatusResponse ToResponse(this PaymentStatus paymentStatus)
        {
            return new PaymentStatusResponse(
                paymentStatus.Id,
                paymentStatus.Description
            );
        }
    }
}
