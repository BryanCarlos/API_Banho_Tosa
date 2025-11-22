using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.ServiceStatuses.DTOs;
using API_Banho_Tosa.Application.ServiceStatuses.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;

namespace API_Banho_Tosa.Application.ServiceStatuses.Services
{
    public class ServiceStatusService(IServiceStatusRepository serviceStatusRepository) : IServiceStatusService
    {
        public async Task<ServiceStatusResponse> CreateServiceStatusAsync(CreateServiceStatusRequest request)
        {
            var treatedDescription = request.Description.Trim();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Service status description cannot be empty.");
            }

            var paymentStatusExist = await serviceStatusRepository.ServiceStatusExistAsync(treatedDescription);

            if (paymentStatusExist)
            {
                throw new DuplicateItemException($"{treatedDescription.ToUpper()} already exist.");
            }

            var serviceStatus = ServiceStatus.Create(request.Description);

            serviceStatusRepository.AddServiceStatus(serviceStatus);
            await serviceStatusRepository.SaveChangesAsync();

            return serviceStatus.ToResponse();
        }

        public async Task DeleteServiceStatusAsync(int id)
        {
            var serviceStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(id);

            if (serviceStatus is null)
            {
                throw new KeyNotFoundException($"Service status with ID {id} doesn't exist.");
            }

            serviceStatusRepository.DeleteServiceStatus(serviceStatus);
            await serviceStatusRepository.SaveChangesAsync();
        }

        public async Task<ServiceStatusResponse> GetServiceStatusByIdAsync(int id)
        {
            var serviceStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(id);

            if (serviceStatus is null)
            {
                throw new KeyNotFoundException($"Service status with ID {id} doesn't exist.");
            }

            return serviceStatus.ToResponse();
        }

        public async Task<IEnumerable<ServiceStatusResponse>> SearchServiceStatusesAsync(ServiceStatusFilterQuery filter)
        {
            var serviceStatuses = await serviceStatusRepository.SearchServiceStatusesAsync(filter.Description);
            return serviceStatuses.Select(ss => ss.ToResponse());
        }

        public async Task<ServiceStatusResponse> UpdateServiceStatusAsync(int id, UpdateServiceStatusRequest request)
        {
            var serviceStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(id);

            if (serviceStatus is null)
            {
                throw new KeyNotFoundException($"Service status with ID {id} doesn't exist.");
            }

            serviceStatus.UpdateDescription(request.Description);
            await serviceStatusRepository.SaveChangesAsync();

            return serviceStatus.ToResponse();
        }
    }
}
