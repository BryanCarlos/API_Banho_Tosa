using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IPaymentStatusRepository
    {
        Task<PaymentStatus?> GetPaymentStatusByIdAsync(int id);
        Task<IEnumerable<PaymentStatus>> SearchPaymentStatusAsync(string? description);
        Task<bool> PaymentStatusExistAsync(string description);
        void AddPaymentStatus(PaymentStatus paymentStatus);
        void DeletePaymentStatus(PaymentStatus paymentStatus);
        Task<int> SaveChangesAsync();
    }
}
