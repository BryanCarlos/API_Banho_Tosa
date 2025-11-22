using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.PaymentStatuses.DTOs;
using API_Banho_Tosa.Application.PaymentStatuses.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.PaymentStatuses.Services
{
    public class PaymentStatusService(IPaymentStatusRepository paymentStatusRepository) : IPaymentStatusService
    {
        public async Task<PaymentStatusResponse> CreatePaymentStatusAsync(CreatePaymentStatusRequest request)
        {
            var treatedDescription = request.Description.Trim();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Payment status description cannot be empty.");
            }

            var paymentStatusExist = await paymentStatusRepository.PaymentStatusExistAsync(treatedDescription);

            if (paymentStatusExist)
            {
                throw new DuplicateItemException($"{treatedDescription.ToUpper()} already exist.");
            }

            var paymentStatus = PaymentStatus.Create(treatedDescription);
            paymentStatusRepository.AddPaymentStatus(paymentStatus);
            await paymentStatusRepository.SaveChangesAsync();

            return paymentStatus.ToResponse();
        }

        public async Task DeletePaymentStatusByIdAsync(int id)
        {
            var paymentStatusToDelete = await paymentStatusRepository.GetPaymentStatusByIdAsync(id);

            if (paymentStatusToDelete is null)
            {
                throw new KeyNotFoundException($"Payment status with ID {id} doesn`t exist.");
            }

            paymentStatusRepository.DeletePaymentStatus(paymentStatusToDelete);
            await paymentStatusRepository.SaveChangesAsync();
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusByIdAsync(int id)
        {
            var paymentStatus = await paymentStatusRepository.GetPaymentStatusByIdAsync(id);

            if (paymentStatus is null)
            {
                throw new KeyNotFoundException($"Payment status with ID {id} doesn`t exist.");
            }

            return paymentStatus.ToResponse();
        }

        public async Task<IEnumerable<PaymentStatusResponse>> SearchPaymentStatusesAsync(string? description)
        {
            var paymentStatuses = await paymentStatusRepository.SearchPaymentStatusAsync(description);
            return paymentStatuses.Select(ps => ps.ToResponse());
        }

        public async Task<PaymentStatusResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request)
        {
            var paymentStatus = await paymentStatusRepository.GetPaymentStatusByIdAsync(id);

            if (paymentStatus is null)
            {
                throw new KeyNotFoundException($"Payment status with ID {id} doesn`t exist.");
            }

            paymentStatus.UpdateDescription(request.Description);
            await paymentStatusRepository.SaveChangesAsync();

            return paymentStatus.ToResponse();
        }
    }
}
