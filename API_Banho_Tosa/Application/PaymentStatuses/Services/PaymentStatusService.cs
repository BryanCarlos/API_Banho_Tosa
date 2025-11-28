using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.PaymentStatuses.DTOs;
using API_Banho_Tosa.Application.PaymentStatuses.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.PaymentStatuses.Services
{
    public class PaymentStatusService(
        IPaymentStatusRepository paymentStatusRepository,
        ICurrentUserService currentUserService,
        ILogger<PaymentStatusService> logger) : IPaymentStatusService
    {
        public async Task<PaymentStatusResponse> CreatePaymentStatusAsync(CreatePaymentStatusRequest request)
        {
            var treatedDescription = request.Description.Trim();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Payment status description cannot be empty.");
            }

            logger.LogInformation("Attempting to create a new payment status with data: {@CreatePaymentStatusRequest}", request);

            var paymentStatusExist = await paymentStatusRepository.PaymentStatusExistAsync(treatedDescription);

            if (paymentStatusExist)
            {
                logger.LogWarning("Attempting to create a payment status {PaymentStatusName} that already exist.", treatedDescription.ToUpper());
                throw new DuplicateItemException($"{treatedDescription.ToUpper()} already exist.");
            }

            var paymentStatus = PaymentStatus.Create(treatedDescription);
            paymentStatusRepository.AddPaymentStatus(paymentStatus);
            await paymentStatusRepository.SaveChangesAsync();

            logger.LogInformation(
                "New payment status '{PaymentStatusName}' with ID {PaymentStatusId} was created by {RequestingUserId} (Name: {RequestingUsername}",
                paymentStatus.Description,
                paymentStatus.Id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );

            return paymentStatus.ToResponse();
        }

        public async Task DeletePaymentStatusByIdAsync(int id)
        {
            var paymentStatusToDelete = await paymentStatusRepository.GetPaymentStatusByIdAsync(id);

            if (paymentStatusToDelete is null)
            {
                logger.LogWarning("Attempted to delete a payment status with ID {PaymentStatusId} that was not found.", id);
                throw new KeyNotFoundException($"Payment status with ID {id} doesn`t exist.");
            }

            paymentStatusRepository.DeletePaymentStatus(paymentStatusToDelete);
            await paymentStatusRepository.SaveChangesAsync();

            logger.LogInformation(
                "Payment status '{PaymentStatusName}' with ID {PaymentStatusId} deleted successfully by user {RequestingUserId} (Name: {RequestingUsername}).",
                paymentStatusToDelete.Description,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusByIdAsync(int id)
        {
            var paymentStatus = await paymentStatusRepository.GetPaymentStatusByIdAsync(id);

            if (paymentStatus is null)
            {
                logger.LogWarning("Attempted to get a payment status info with ID {PaymentStatusId} that was not found.", id);
                throw new KeyNotFoundException($"Payment status with ID {id} doesn`t exist.");
            }

            return paymentStatus.ToResponse();
        }

        public async Task<IEnumerable<PaymentStatusResponse>> SearchPaymentStatusesAsync(string? description)
        {
            var paymentStatuses = await paymentStatusRepository.SearchPaymentStatusAsync(description);

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(description))
            {
                criteria.Add("name like {PaymentStatusName}");
                logArgs.Add(description);
            }

            var logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all payment status found {PaymentStatusCount} results.";
            }
            else
            {
                logMessage = $"Search for apayment status with {string.Join(" and ", criteria)} found {{PaymentStatusCount}} results.";
            }

            logger.LogInformation(logMessage, logArgs);

            return paymentStatuses.Select(ps => ps.ToResponse());
        }

        public async Task<PaymentStatusResponse> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request)
        {
            var paymentStatus = await paymentStatusRepository.GetPaymentStatusByIdAsync(id);

            if (paymentStatus is null)
            {
                logger.LogWarning("Attempted to update a payment status info with ID {PaymentStatusId} that was not found.", id);
                throw new KeyNotFoundException($"Payment status with ID {id} doesn`t exist.");
            }

            var oldData = new
            {
                Description = paymentStatus.Description
            };

            var newData = new
            {
                Description = request.Description
            };

            paymentStatus.UpdateDescription(request.Description);
            await paymentStatusRepository.SaveChangesAsync();

            logger.LogInformation(
                "Payment status {PaymentStatusName} with ID {PaymentStatusId} updated successfully by user {RequestingUserId} (Name: {RequestingUserName}). Changes {@Changes}",
                paymentStatus.Description,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A",
                new { Old = oldData, New = newData }
            );

            return paymentStatus.ToResponse();
        }
    }
}
