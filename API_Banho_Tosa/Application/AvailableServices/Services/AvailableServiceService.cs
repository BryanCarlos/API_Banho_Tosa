using API_Banho_Tosa.API.Services;
using API_Banho_Tosa.Application.AvailableServices.DTOs;
using API_Banho_Tosa.Application.AvailableServices.Mappers;
using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;

namespace API_Banho_Tosa.Application.AvailableServices.Services
{
    public class AvailableServiceService(
        IAvailableServiceRepository availableServiceRepository,
        ICurrentUserService currentUserService,
        ILogger<AvailableServiceService> logger) : IAvailableServiceService
    {
        public async Task<AvailableServiceResponse> CreateAvailableServiceAsync(CreateAvailableServiceRequest request)
        {
            var treatedDescription = request.Description?.Trim();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Description cannot be empty.");
            }

            logger.LogInformation("Attempting to create a new available service with data: {@CreateAvailableServiceRequest}", request);

            var serviceExist = await availableServiceRepository.AvailableServiceExistAsync(treatedDescription);

            if (serviceExist)
            {
                logger.LogWarning("Attempting to create a available service {AvailableServiceName} that already exist.", treatedDescription.ToUpper());
                throw new DuplicateItemException($"{treatedDescription.ToUpper()} already exist.");
            }

            var availableService = AvailableService.Create(treatedDescription, request.DurationInMinutes);
            availableServiceRepository.AddAvailableService(availableService);
            await availableServiceRepository.SaveChangesAsync();

            logger.LogInformation(
                "New available service '{AvailableServiceName}' with ID {AvailableServiceId} was created by {RequestingUserId} (Name: {RequestingUsername}",
                availableService.Description,
                availableService.Uuid,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );

            return availableService.ToResponse();
        }

        public async Task DeleteAvailableServiceAsync(Guid id)
        {
            var availableService = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                logger.LogWarning("Attempted to delete an available service with ID {AvailableServiceId} that was not found.", id);
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            availableService.Delete();
            await availableServiceRepository.SaveChangesAsync();

            logger.LogInformation(
                "Available service '{AvailableServiceName}' with ID {AvailableServiceId} deleted successfully by user {RequestingUserId} (Name: {RequestingUsername}).",
                availableService.Description,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task<AvailableServiceResponse> GetAvailableServiceByUuidAsync(Guid id)
        {
            var availableService = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                logger.LogWarning("Attempted to get an available service info with ID {AvailableServiceId} that was not found.", id);
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            return availableService.ToResponse();
        }

        public async Task ReactivateAvailableServiceAsync(Guid id)
        {
            var availableService = await availableServiceRepository.GetDeletedAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                logger.LogWarning("Attempted to reactivate an available service info with ID {AvailableServiceId} that was not found.", id);
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            availableService.Reactivate();
            await availableServiceRepository.SaveChangesAsync();

            logger.LogInformation(
                "Service {AvailableServiceName} (ID: {AvailableServiceId} reactivated by user {RequestingUserId} (Name: {RequestingUsername}",
                availableService.Description,
                availableService.Uuid,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task<IEnumerable<AvailableServiceResponse>> SearchAvailableServicesAsync(AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceRepository.SearchAvailableServicesAsync(filter.Description);

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(filter.Description))
            {
                criteria.Add("name like {AvailableServiceName}");
                logArgs.Add(filter.Description);
            }

            var logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all available services found {AvailableServicesCount} results.";
            }
            else
            {
                logMessage = $"Search for available services with {string.Join(" and ", criteria)} found {{AvailableServicesCount}} results.";
            }

            logger.LogInformation(logMessage, logArgs);

            return response.Select(s => s.ToResponse());
        }

        public async Task<IEnumerable<AvailableServiceResponse>> SearchDeletedAvailableServicesAsync(AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceRepository.SearchDeletedAvailableServicesAsync(filter.Description);

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(filter.Description))
            {
                criteria.Add("name like {AvailableServiceName}");
                logArgs.Add(filter.Description);
            }

            var logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all deleted available services found {AvailableServicesCount} results.";
            }
            else
            {
                logMessage = $"Search for deleted available services with {string.Join(" and ", criteria)} found {{AvailableServicesCount}} results.";
            }

            logger.LogInformation(logMessage, logArgs);

            return response.Select(s => s.ToResponse());
        }

        public async Task<AvailableServiceResponse> UpdateAvailableServiceAsync(Guid id, UpdateAvailableServiceRequest request)
        {
            var availableService = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                logger.LogWarning("Attempted to update an available service info with ID {AvailableServiceId} that was not found.", id);
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            var oldData = new
            {
                Description = availableService.Description,
                DurationInMinutes = availableService.ServiceDurationMinutes
            };

            var newData = new
            {
                Description = request.Description,
                DurationInMinutes = request.DurationInMinutes
            };
           
            availableService.UpdateDescription(request.Description);
            availableService.UpdateServiceMinutes(request.DurationInMinutes);
            await availableServiceRepository.SaveChangesAsync();

            logger.LogInformation(
                "Available service {AvailableServiceName} with ID {AvailableServiceId} updated successfully by user {RequestingUserId} (Name: {RequestingUserName}). Changes {@Changes}",
                availableService.Description,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A",
                new { Old = oldData, New = newData }
            );

            return availableService.ToResponse();
        }
    }
}
