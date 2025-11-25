using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.ServicePrices.DTOs;
using API_Banho_Tosa.Application.ServicePrices.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.ServicePrices.Services
{
    public class ServicePriceService(
        IServicePriceRepository servicePriceRepository,
        IAvailableServiceRepository availableServiceRepository,
        IPetSizeRepository petSizeRepository) : IServicePriceService
    {
        public async Task<ServicePriceResponse> AddServicePriceAsync(Guid id, AddServicePriceRequest request)
        {
            var serviceAvailable = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);
            var petSizeExist = await petSizeRepository.GetPetSizeByIdAsync(request.PetSizeId);

            var errorMessage = new List<string>();

            if (serviceAvailable is null)
            {
                errorMessage.Add($"Available service with ID {id} doesn't exist.");
            }
            if (petSizeExist is null)
            {
                errorMessage.Add($"Pet size with ID {request.PetSizeId} doesn't exist.");
            }

            if (errorMessage.Count > 0)
            {
                throw new KeyNotFoundException(string.Join(" | ", errorMessage));
            }

            var existingPrice = await servicePriceRepository.GetServicePriceByCompositeKeyAsync(serviceAvailable!.Id, request.PetSizeId);
            if (existingPrice != null)
            {
                throw new DuplicateItemException("There is already a set price for this service and size.");
            }

            var servicePrice = ServicePrice.Create(serviceAvailable!.Id, request.PetSizeId, request.Price);
            servicePriceRepository.AddServicePrice(servicePrice);
            await servicePriceRepository.SaveChangesAsync();

            servicePrice.UpdateAvailableService(serviceAvailable);
            servicePrice.UpdatePetSize(petSizeExist!);

            return servicePrice.ToResponse();
        }

        public async Task<ServicePriceResponse> CreateServicePriceAsync(CreateServicePriceRequest request)
        {
            var servicePriceExist = await servicePriceRepository.ServicePriceExistAsync(request.AvailableServiceId, request.PetSizeId);

            if (servicePriceExist)
            {
                throw new DuplicateItemException($"Service price already defined for service with ID {request.AvailableServiceId} and pet size with ID {request.PetSizeId}.");
            }

            var availableServiceExist = await availableServiceRepository.GetAvailableServiceByIdAsync(request.AvailableServiceId);
            var petSizeExist = await petSizeRepository.GetPetSizeByIdAsync(request.PetSizeId);

            var errorMessage = new List<string>();

            if (availableServiceExist is null)
            {
                errorMessage.Add($"Available service with ID {request.AvailableServiceId} doesn't exist.");
            }
            if (petSizeExist is null)
            {
                errorMessage.Add($"Pet size with ID {request.PetSizeId} doesn't exist.");
            }

            if (errorMessage.Count > 0)
            {
                throw new KeyNotFoundException(string.Join(" | ", errorMessage));
            }

            var servicePrice = ServicePrice.Create(request.AvailableServiceId, request.PetSizeId, request.ServicePrice);

            servicePriceRepository.AddServicePrice(servicePrice);
            await servicePriceRepository.SaveChangesAsync();

            servicePrice.UpdateAvailableService(availableServiceExist!);
            servicePrice.UpdatePetSize(petSizeExist!);

            return servicePrice.ToResponse();
        }

        public async Task DeleteServicePriceAsync(int serviceId, int petSizeId)
        {
            var servicePrice = await servicePriceRepository.GetServicePriceByCompositeKeyAsync(serviceId, petSizeId);

            if (servicePrice is null)
            {
                throw new KeyNotFoundException($"Service price for service with ID {serviceId} and pet size {petSizeId} doesn't exist.");
            }

            servicePriceRepository.DeleteServicePrice(servicePrice);
            await servicePriceRepository.SaveChangesAsync();
        }

        public async Task DeleteServicePriceAsync(Guid serviceId, int petSizeId)
        {
            var serviceAvailable = await availableServiceRepository.GetAvailableServiceByUuidAsync(serviceId);

            if (serviceAvailable is null)
            {
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            var servicePrice = await servicePriceRepository.GetServicePriceByCompositeKeyAsync(serviceAvailable.Id, petSizeId);

            if (servicePrice is null)
            {
                throw new KeyNotFoundException($"Price is not set for service with ID {serviceId.ToString().Substring(0, 8)} pet size with ID {petSizeId}.");
            }

            servicePriceRepository.DeleteServicePrice(servicePrice);
            await servicePriceRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ServicePriceResponse>> GetServicePricesByServiceAsync(Guid serviceId)
        {
            var servicePrices = await servicePriceRepository.GetAllByServiceUuidAsync(serviceId);
            return servicePrices.Select(sp => sp.ToResponse());
        }

        public async Task<ServicePriceResponse> UpdateServicePriceAsync(UpdateServicePriceRequest request)
        {
            var servicePrice = await servicePriceRepository.GetServicePriceByCompositeKeyAsync(request.AvailableServiceId, request.PetSizeId);

            if (servicePrice is null)
            {
                throw new KeyNotFoundException($"Service price for service with ID {request.AvailableServiceId} and pet size {request.PetSizeId} doesn't exist.");
            }

            servicePrice.UpdatePrice(request.Price);
            await servicePriceRepository.SaveChangesAsync();

            return servicePrice.ToResponse();
        }

        public async Task<ServicePriceResponse> UpdateServicePriceAsync(Guid id, int petSizeId, UpdatePriceRequest request)
        {
            var serviceAvailable = await availableServiceRepository.GetAvailableServiceByUuidAsync(id);

            if (serviceAvailable is null)
            {
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            var servicePrice = await servicePriceRepository.GetServicePriceByCompositeKeyAsync(serviceAvailable.Id, petSizeId);

            if (servicePrice is null)
            {
                throw new KeyNotFoundException($"Price is not set for service with ID {id.ToString().Substring(0, 8)} pet size with ID {petSizeId}.");
            }

            servicePrice.UpdatePrice(request.Price);
            await servicePriceRepository.SaveChangesAsync();

            return servicePrice.ToResponse();
        }
    }
}
