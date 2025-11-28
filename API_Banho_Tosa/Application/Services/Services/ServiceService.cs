using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.Services.DTOs;
using API_Banho_Tosa.Application.Services.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Enums;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.Services.Services
{
    public class ServiceService(
        IServiceRepository serviceRepository,
        IPetRepository petRepository,
        IServiceStatusRepository serviceStatusRepository,
        IPaymentStatusRepository paymentStatusRepository,
        IServicePriceRepository servicePriceRepository,
        ICurrentUserService currentUserService,
        ILogger<ServiceService> logger) : IServiceService
    {
        public async Task<ServiceResponse> CreateServiceAsync(CreateServiceRequest request)
        {
            var pet = await petRepository.GetPetByIdAsync(request.PetId);

            if (pet is null)
            {
                logger.LogWarning("Attempted to get pet info with ID {PetId} that was not found.", request.PetId.ToString());
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            var serviceItems = new List<ServiceItem>();

            var groupedRequests = request.AvailableServicesId
                .GroupBy(id => id)
                .Select(g => new { ServiceId = g.Key, Quantity = g.Count() });

            foreach (var itemRequest in groupedRequests)
            {
                var priceEntity = await servicePriceRepository.GetServicePriceByCompositeKeyAsync(itemRequest.ServiceId, pet.PetSizeId);

                if (priceEntity is null)
                {
                    logger.LogWarning(
                        "There is no price configured for the service {ServiceAvailableId} in size {PetSizeDescription}.",
                        itemRequest.ServiceId,
                        pet.PetSize.Description
                    );
                    throw new InvalidOperationException($"There is no price configured for the service {itemRequest.ServiceId} in size {pet.PetSize.Description}.");
                }

                var item = ServiceItem.Create(
                    availableServiceId: itemRequest.ServiceId,
                    price: priceEntity.Price,
                    quantity: itemRequest.Quantity
                );
                serviceItems.Add(item);
            }

            var status = await serviceStatusRepository.GetServiceStatusByIdAsync((int)ServiceStatusEnum.Agendado);
            var paymentStatus = await paymentStatusRepository.GetPaymentStatusByIdAsync((int)PaymentStatusEnum.Pendente);

            var service = Service.Create(request.PetId, request.ServiceDate, serviceItems, request.DiscountValue ?? 0, request.AdditionalCharges ?? 0);
            service.SetStatus(status!);
            service.SetPaymentStatus(paymentStatus!);

            serviceRepository.AddService(service);
            await serviceRepository.SaveChangesAsync();

            logger.LogInformation(
                "New service created for pet {PetName} costing {ServiceCost} by user {RequestingUserId} (Name: {RequestingUsername})",
                pet.Name,
                service.FinalTotal,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );

            return service.ToResponse();
        }

        public async Task DeleteServiceAsync(Guid id)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);

            if (service is null)
            {
                logger.LogWarning("Attempted to delete a service with ID {ServiceId} that was not found.", id.ToString());
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            service.Delete();
            await serviceRepository.SaveChangesAsync();

            logger.LogInformation(
                "Service with ID {ServiceId} deleted successfully by user {RequestingUserId} (Name: {RequestingUsername})",
                id.ToString(),
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task<ServiceResponse> GetServiceByUuidAsync(Guid id)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);

            if (service is null)
            {
                logger.LogWarning("Attempted to get service info with ID {ServiceId} that was not found.", id.ToString());
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            return service.ToResponse();
        }

        public async Task ReactivateServiceAsync(Guid id)
        {
            var service = await serviceRepository.GetDeletedServiceByUuidAsync(id);

            if (service is null)
            {
                logger.LogWarning("Attempted to reactivate a service with ID {ServiceId} that was not found.", id.ToString());
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            service.Reactivate();
            await serviceRepository.SaveChangesAsync();

            logger.LogInformation(
                "Service {ServiceId} reactivated by user {RequestingUserId} (Name: {RequestingUsername}",
                id.ToString(),
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task<IEnumerable<ServiceResponse>> SearchDeletedServicesAsync(ServiceFilterQuery filter)
        {
            var services = await serviceRepository.SearchDeletedServicesAsync(
                filter.StartDate,
                filter.EndDate,
                filter.StatusId,
                filter.PaymentStatusId,
                filter.PetId,
                filter.OwnerId
            );

            var criteria = new List<string>();
            var logArgs = new List<string>();

            if (filter.StartDate.HasValue)
            {
                logArgs.Add(filter.StartDate.Value.ToString());
                criteria.Add("starts at {StartDate}");
            }
            if (filter.EndDate.HasValue)
            {
                logArgs.Add(filter.EndDate.Value.ToString());
                criteria.Add("ends at {EndDate}");
            }
            if (filter.StatusId.HasValue)
            {
                logArgs.Add(filter.StatusId.Value.ToString());
                criteria.Add("status ID equals to {StatusId}");
            }
            if (filter.PaymentStatusId.HasValue)
            {
                logArgs.Add(filter.PaymentStatusId.Value.ToString());
                criteria.Add("payment status ID equals to {PaymentStatusId}");
            }
            if (filter.PetId.HasValue)
            {
                logArgs.Add(filter.PetId.Value.ToString());
                criteria.Add("pet ID equals to {PetId}");
            }
            if (filter.OwnerId.HasValue)
            {
                logArgs.Add(filter.OwnerId.Value.ToString());
                criteria.Add("owner ID equals to {OwnerId}");
            }

            var logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logArgs.Add(services.Count().ToString());
                logMessage = "Search for all deleted services returned {ServicesCount} results.";
            }
            else
            {
                logArgs.Add(services.Count().ToString());
                logMessage = $"Search for deleted services where {string.Join(" and ", criteria)} returned {{ServicesCount}} results.";
            }

            logger.LogInformation(logMessage, logArgs);

            return services.Select(s => s.ToResponse());
        }

        public async Task<IEnumerable<ServiceResponse>> SearchServicesAsync(ServiceFilterQuery filter)
        {
            var services = await serviceRepository.SearchServicesAsync(
                filter.StartDate,
                filter.EndDate,
                filter.StatusId,
                filter.PaymentStatusId,
                filter.PetId,
                filter.OwnerId
            );

            var criteria = new List<string>();
            var logArgs = new List<string>();

            if (filter.StartDate.HasValue)
            {
                logArgs.Add(filter.StartDate.Value.ToString());
                criteria.Add("starts at {StartDate}");
            }
            if (filter.EndDate.HasValue)
            {
                logArgs.Add(filter.EndDate.Value.ToString());
                criteria.Add("ends at {EndDate}");
            }
            if (filter.StatusId.HasValue)
            {
                logArgs.Add(filter.StatusId.Value.ToString());
                criteria.Add("status ID equals to {StatusId}");
            }
            if (filter.PaymentStatusId.HasValue)
            {
                logArgs.Add(filter.PaymentStatusId.Value.ToString());
                criteria.Add("payment status ID equals to {PaymentStatusId}");
            }
            if (filter.PetId.HasValue)
            {
                logArgs.Add(filter.PetId.Value.ToString());
                criteria.Add("pet ID equals to {PetId}");
            }
            if (filter.OwnerId.HasValue)
            {
                logArgs.Add(filter.OwnerId.Value.ToString());
                criteria.Add("owner ID equals to {OwnerId}");
            }

            var logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logArgs.Add(services.Count().ToString());
                logMessage = "Search for all services returned {ServicesCount} results.";
            }
            else
            {
                logArgs.Add(services.Count().ToString());
                logMessage = $"Search for services where {string.Join(" and ", criteria)} returned {{ServicesCount}} results.";
            }

            logger.LogInformation(logMessage, logArgs);

            return services.Select(s => s.ToResponse());
        }

        public async Task<ServiceResponse> UpdateServiceAsync(Guid id, UpdateServiceRequest request)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);

            if (service is null)
            {
                logger.LogWarning(
                    "Attempted to update a service with ID {ServiceId} that was not found. User: {RequestingUserId} ({RequestingUserName}).",
                    id,
                    currentUserService.UserId.ToString() ?? "N/A",
                    currentUserService.Username ?? "N/A"
                );
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            if (service.PaymentStatusId == (int)PaymentStatusEnum.Pago)
            {
                logger.LogWarning(
                    "Update blocked for service {ServiceId} because it is already paid. User: {RequestingUserId} ({RequestingUserName}).",
                    id,
                    currentUserService.UserId.ToString() ?? "N/A",
                    currentUserService.Username ?? "N/A"
                );
                throw new InvalidOperationException("It's not possible to change the items of a service that has already been paid for. Cancel and create a new one or get a refund first.");
            }

            if (service.ServiceStatusId == (int)ServiceStatusEnum.Concluido)
            {
                logger.LogWarning(
                    "Update blocked for service {ServiceId} because it is already completed. User: {RequestingUserId} ({RequestingUserName}).",
                    id,
                    currentUserService.UserId.ToString() ?? "N/A",
                    currentUserService.Username ?? "N/A"
                );
                throw new InvalidOperationException("It is not possible to change a service that has already been completed.");
            }

            serviceRepository.DeleteServiceItems(service.ServiceItems);

            var serviceItems = new List<ServiceItem>();

            var groupedRequests = request.AvailableServicesId
                .GroupBy(id => id)
                .Select(g => new { ServiceId = g.Key, Quantity = g.Count() });

            foreach (var itemRequest in groupedRequests)
            {
                var priceEntity = await servicePriceRepository.GetServicePriceByCompositeKeyAsync(itemRequest.ServiceId, service.Pet.PetSizeId);

                if (priceEntity is null)
                {
                    logger.LogError(
                        "Configuration Error: Missing price for Service {AvailableServiceId} and PetSize {PetSize} (ID: {PetSizeId}) during update of Service {ServiceId}.",
                        itemRequest.ServiceId,
                        service.Pet.PetSize.Description,
                        service.Pet.PetSizeId,
                        id
                    );
                    throw new InvalidOperationException($"There is no price configured for the service {itemRequest.ServiceId} in size {service.Pet.PetSize.Description}.");
                }

                var item = ServiceItem.Create(
                    availableServiceId: itemRequest.ServiceId,
                    price: priceEntity.Price,
                    quantity: itemRequest.Quantity
                );
                serviceItems.Add(item);
            }

            var oldData = new
            {
                ServiceDate = service.ServiceDate,
                ItemsCount = serviceItems.Count,
                FinalTotal = service.FinalTotal
            };

            service.Update(request.ServiceDate, serviceItems, request.DiscountValue, request.AdditionalCharges);
            await serviceRepository.SaveChangesAsync();

            var newData = new
            {
                ServiceDate = service.ServiceDate,
                ItemsCount = serviceItems.Count,
                FinalTotal = service.FinalTotal
            };

            logger.LogInformation(
                "Service {ServiceId} updated successfully. Updated by user {RequestingUserId} ({RequestingUserName}). Changes: {@Changes}. ",
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A",
                new { Old = oldData, New = newData }
            );

            return service.ToResponse();
        }

        public async Task<ServiceResponse> UpdatePaymentStatusAsync(Guid id, int newPaymentStatusId)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);
            if (service is null)
            {
                logger.LogWarning(
                    "Attempted to update payment status for service {ServiceId} that was not found. User: {RequestingUserId} ({RequestingUserName}).",
                    id,
                    currentUserService.UserId.ToString() ?? "N/A",
                    currentUserService.Username ?? "N/A"
                );
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            var newStatus = await paymentStatusRepository.GetPaymentStatusByIdAsync(newPaymentStatusId);
            if (newStatus is null)
            {
                logger.LogWarning(
                    "Attempted to set invalid payment status ID {PaymentStatusId} for service {ServiceId}. User: {RequestingUserId} ({RequestingUserName}).",
                    newPaymentStatusId,
                    id,
                    currentUserService.UserId.ToString() ?? "N/A",
                    currentUserService.Username ?? "N/A"
                );
                throw new KeyNotFoundException($"Payment status with ID '{newPaymentStatusId}' doesn't exist.");
            }

            if (newPaymentStatusId == (int)PaymentStatusEnum.Pago)
            {
                service.SetPaymentDate(DateTime.UtcNow);
            }
            else
            {
                service.SetPaymentDate(null);
            }

            service.SetPaymentStatus(newStatus);

            await serviceRepository.SaveChangesAsync();

            logger.LogInformation(
                "Service {ServiceId} payment status updated to '{PaymentStatusDescription}' (ID: {PaymentStatusId}). Payment Date: {PaymentDate}. Updated by user {RequestingUserId} ({RequestingUserName}).",
                id,
                newStatus.Description,
                newStatus.Id,
                service.PaymentDate.HasValue ? service.PaymentDate.ToString() : "Cleared",
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );

            return service.ToResponse();
        }

        public async Task<ServiceResponse> UpdateServiceStatusAsync(Guid id, int newStatusId)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);
            if (service is null)
            {
                logger.LogWarning(
                    "Attempted to update service status for service {ServiceId} that was not found. User: {RequestingUserId} ({RequestingUserName}).",
                    id,
                    currentUserService.UserId.ToString() ?? "N/A",
                    currentUserService.Username ?? "N/A"
                );
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            var newStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(newStatusId);
            if (newStatus is null)
            {
                logger.LogWarning(
                    "Attempted to set invalid service status ID {ServiceStatusId} for service {ServiceId}. User: {RequestingUserId} ({RequestingUserName}).",
                    newStatusId,
                    id,
                    currentUserService.UserId.ToString() ?? "N/A",
                    currentUserService.Username ?? "N/A"
                );
                throw new KeyNotFoundException($"Service status with ID {newStatusId} doesn't exist.");
            }

            service.SetStatus(newStatus);

            await serviceRepository.SaveChangesAsync();

            logger.LogInformation(
                "Service {ServiceId} status updated to '{ServiceStatusDescription}' (ID: {ServiceStatusId}). Updated by user {RequestingUserId} ({RequestingUserName}).",
                id,
                newStatus.Description,
                newStatus.Id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );

            return service.ToResponse();
        }
    }
}
