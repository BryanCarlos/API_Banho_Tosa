using API_Banho_Tosa.Application.PaymentStatuses.DTOs;

namespace API_Banho_Tosa.Application.PaymentStatuses.Services
{
    public interface IPaymentStatusService
    {
        Task<PaymentStatusResponse> CreatePaymentStatusAsync(CreatePaymentStatusRequest request);
        Task<PaymentStatusResponse> GetPaymentStatusByIdAsync(int id);
        Task<IEnumerable<PaymentStatusResponse>> SearchPaymentStatusesAsync(string? description);
        Task<PaymentStatusResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request);
        Task DeletePaymentStatusByIdAsync(int id);
    }
}
