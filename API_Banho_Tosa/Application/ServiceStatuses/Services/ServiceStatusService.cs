using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.ServiceStatuses.DTOs;
using API_Banho_Tosa.Application.ServiceStatuses.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;

namespace API_Banho_Tosa.Application.ServiceStatuses.Services
{
    public class ServiceStatusService(
        IServiceStatusRepository serviceStatusRepository,
        ICurrentUserService currentUserService,
        ILogger<ServiceStatusService> logger) : IServiceStatusService
    {
        public async Task<ServiceStatusResponse> CreateServiceStatusAsync(CreateServiceStatusRequest request)
        {
            var treatedDescription = request.Description.Trim();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Service status description cannot be empty.");
            }

            logger.LogInformation("Attempting to create a new service status with data: {@CreateServiceStatusRequest}", request);

            var paymentStatusExist = await serviceStatusRepository.ServiceStatusExistAsync(treatedDescription);

            if (paymentStatusExist)
            {
                logger.LogWarning("Attempting to create a service status {ServiceStatusName} that already exist.", treatedDescription.ToUpper());
                throw new DuplicateItemException($"{treatedDescription.ToUpper()} already exist.");
            }

            var serviceStatus = ServiceStatus.Create(request.Description);

            serviceStatusRepository.AddServiceStatus(serviceStatus);
            await serviceStatusRepository.SaveChangesAsync();

            logger.LogInformation(
               "New service status '{ServiceStatusName}' with ID {ServiceStatusId} was created by {RequestingUserId} (Name: {RequestingUsername}",
               serviceStatus.Description,
               serviceStatus.Id,
               currentUserService.UserId.ToString() ?? "N/A",
               currentUserService.Username ?? "N/A"
           );

            return serviceStatus.ToResponse();
        }

        public async Task DeleteServiceStatusAsync(int id)
        {
            var serviceStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(id);

            if (serviceStatus is null)
            {
                logger.LogWarning("Attempted to delete a service status with ID {ServiceStatusId} that was not found.", id);
                throw new KeyNotFoundException($"Service status with ID {id} doesn't exist.");
            }

            serviceStatusRepository.DeleteServiceStatus(serviceStatus);
            await serviceStatusRepository.SaveChangesAsync();

            logger.LogInformation(
                "Service status '{ServiceStatusName}' with ID {ServiceStatusId} deleted successfully by user {RequestingUserId} (Name: {RequestingUsername}).",
                serviceStatus.Description,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task<ServiceStatusResponse> GetServiceStatusByIdAsync(int id)
        {
            var serviceStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(id);

            if (serviceStatus is null)
            {
                logger.LogWarning("Attempted to get a service status info with ID {ServiceStatusId} that was not found.", id);
                throw new KeyNotFoundException($"Service status with ID {id} doesn't exist.");
            }

            return serviceStatus.ToResponse();
        }

        public async Task<IEnumerable<ServiceStatusResponse>> SearchServiceStatusesAsync(ServiceStatusFilterQuery filter)
        {
            var serviceStatuses = await serviceStatusRepository.SearchServiceStatusesAsync(filter.Description);

            var criteria = new List<string>();
            var logArgs = new List<string>();
            var logMessage = string.Empty;

            if (!string.IsNullOrWhiteSpace(filter.Description))
            {
                criteria.Add("description like {ServiceStatusDescription}");
                logArgs.Add(filter.Description);
            }

            if (criteria.Count == 0)
            {
                logMessage = "Search for all service statuses retuned {ServiceStatusCount} results.";
                logArgs.Add(serviceStatuses.Count().ToString());
            }
            else
            {
                logMessage = $"Search for services status with {string.Join(" and ", criteria)}.";
                logArgs.Add(serviceStatuses.Count().ToString());
            }

            logger.LogInformation(logMessage, logArgs);

            return serviceStatuses.Select(ss => ss.ToResponse());
        }

        public async Task<ServiceStatusResponse> UpdateServiceStatusAsync(int id, UpdateServiceStatusRequest request)
        {
            var serviceStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(id);

            if (serviceStatus is null)
            {
                logger.LogWarning("Attempted to update a service status info with ID {ServiceStatusId} that was not found.", id);
                throw new KeyNotFoundException($"Service status with ID {id} doesn't exist.");
            }

            serviceStatus.UpdateDescription(request.Description);
            await serviceStatusRepository.SaveChangesAsync();

            var oldData = new
            {
                Description = serviceStatus.Description
            };

            var newData = new
            {
                Description = request.Description
            };

            logger.LogInformation(
                "Service status {ServiceStatusName} with ID {ServiceStatusId} updated successfully by user {RequestingUserId} (Name: {RequestingUserName}). Changes {@Changes}",
                serviceStatus.Description,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A",
                new { Old = oldData, New = newData }
            );

            return serviceStatus.ToResponse();
        }
    }
}
