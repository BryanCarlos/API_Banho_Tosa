using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class PaymentStatusRepository(BanhoTosaContext context) : IPaymentStatusRepository
    {
        public void AddPaymentStatus(PaymentStatus paymentStatus)
        {
            context.PaymentStatuses.Add(paymentStatus);
        }

        public void DeletePaymentStatus(PaymentStatus paymentStatus)
        {
            context.PaymentStatuses.Remove(paymentStatus);
        }

        public async Task<PaymentStatus?> GetPaymentStatusByIdAsync(int id)
        {
            return await context.PaymentStatuses.FindAsync(id);
        }

        public async Task<bool> PaymentStatusExistAsync(string description)
        {
            return await context.PaymentStatuses.AnyAsync(ps => EF.Functions.ILike(ps.Description, description));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PaymentStatus>> SearchPaymentStatusAsync(string? description)
        {
            var query = context.PaymentStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(ps => EF.Functions.ILike(ps.Description, $"{description.Trim()}%"));
            }

            return await query.ToListAsync();
        }
    }
}
