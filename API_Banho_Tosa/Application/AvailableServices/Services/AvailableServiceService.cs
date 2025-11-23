using API_Banho_Tosa.Application.AvailableServices.DTOs;
using API_Banho_Tosa.Application.AvailableServices.Mappers;
using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.AvailableServices.Services
{
    public class AvailableServiceService(IAvailableServiceRepository availableServiceRepository) : IAvailableServiceService
    {
        public async Task<AvailableServiceResponse> CreateAvailableServiceAsync(CreateAvailableServiceRequest request)
        {
            var treatedDescription = request.Description?.Trim();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Description cannot be empty.");
            }

            var serviceExist = await availableServiceRepository.AvailableServiceExistAsync(treatedDescription);

            if (serviceExist)
            {
                throw new DuplicateItemException($"{treatedDescription.ToUpper()} already exist.");
            }

            var availableService = AvailableService.Create(treatedDescription, request.DurationInMinutes);
            availableServiceRepository.AddAvailableService(availableService);
            await availableServiceRepository.SaveChangesAsync();

            return availableService.ToResponse();
        }

        public async Task DeleteAvailableServiceAsync(Guid id)
        {
            var availableService = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            availableService.Delete();
            await availableServiceRepository.SaveChangesAsync();
        }

        public async Task<AvailableServiceResponse> GetAvailableServiceByUuidAsync(Guid id)
        {
            var availableService = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            return availableService.ToResponse();
        }

        public async Task ReactivateAvailableServiceAsync(Guid id)
        {
            var availableService = await availableServiceRepository.GetDeletedAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            availableService.Reactivate();
            await availableServiceRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AvailableServiceResponse>> SearchAvailableServicesAsync(AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceRepository.SearchAvailableServicesAsync(filter.Description);
            return response.Select(s => s.ToResponse());
        }

        public async Task<IEnumerable<AvailableServiceResponse>> SearchDeletedAvailableServicesAsync(AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceRepository.SearchDeletedAvailableServicesAsync(filter.Description);
            return response.Select(s => s.ToResponse());
        }

        public async Task<AvailableServiceResponse> UpdateAvailableServiceAsync(Guid id, UpdateAvailableServiceRequest request)
        {
            var availableService = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);

            if (availableService is null)
            {
                throw new KeyNotFoundException("Available service doesn't exist.");
            }

            availableService.UpdateDescription(request.Description);
            availableService.UpdateServiceMinutes(request.DurationInMinutes);
            await availableServiceRepository.SaveChangesAsync();

            return availableService.ToResponse();
        }
    }
}
