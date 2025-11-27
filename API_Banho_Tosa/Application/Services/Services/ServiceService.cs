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
        IServicePriceRepository servicePriceRepository) : IServiceService
    {
        public async Task<ServiceResponse> CreateServiceAsync(CreateServiceRequest request)
        {
            var pet = await petRepository.GetPetByIdAsync(request.PetId);

            if (pet is null)
            {
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

            return service.ToResponse();
        }

        public async Task DeleteServiceAsync(Guid id)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);

            if (service is null)
            {
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            service.Delete();
            await serviceRepository.SaveChangesAsync();
        }

        public async Task<ServiceResponse> GetServiceByUuidAsync(Guid id)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);

            if (service is null)
            {
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            return service.ToResponse();
        }

        public async Task ReactivateServiceAsync(Guid id)
        {
            var service = await serviceRepository.GetDeletedServiceByUuidAsync(id);

            if (service is null)
            {
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            service.Reactivate();
            await serviceRepository.SaveChangesAsync();
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

            return services.Select(s => s.ToResponse());
        }

        public async Task<ServiceResponse> UpdateServiceAsync(Guid id, UpdateServiceRequest request)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);
            if (service is null)
            {
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            if (service.PaymentStatusId == (int)PaymentStatusEnum.Pago)
            {
                throw new InvalidOperationException("It's not possible to change the items of a service that has already been paid for. Cancel and create a new one or get a refund first.");
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
                    throw new InvalidOperationException($"There is no price configured for the service {itemRequest.ServiceId} in size {service.Pet.PetSize.Description}.");
                }
                if (service.ServiceStatusId == (int)ServiceStatusEnum.Concluido)
                {
                    throw new InvalidOperationException("It is not possible to change a service that has already been completed.");
                }

                var item = ServiceItem.Create(
                    availableServiceId: itemRequest.ServiceId,
                    price: priceEntity.Price,
                    quantity: itemRequest.Quantity
                );
                serviceItems.Add(item);
            }

            service.Update(request.ServiceDate, serviceItems, request.DiscountValue, request.AdditionalCharges);
            await serviceRepository.SaveChangesAsync();

            return service.ToResponse();
        }

        public async Task<ServiceResponse> UpdatePaymentStatusAsync(Guid id, int newPaymentStatusId)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);
            if (service is null)
            {
                throw new KeyNotFoundException("Service doesn't exist.");
            }

            var newStatus = await paymentStatusRepository.GetPaymentStatusByIdAsync(newPaymentStatusId);
            if (newStatus is null)
            {
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

            return service.ToResponse();
        }

        public async Task<ServiceResponse> UpdateServiceStatusAsync(Guid id, int newStatusId)
        {
            var service = await serviceRepository.GetServiceByUuidAsync(id);
            if (service is null)
            {
                throw new KeyNotFoundException("Agendamento não encontrado.");
            }

            var newStatus = await serviceStatusRepository.GetServiceStatusByIdAsync(newStatusId);
            if (newStatus is null)
            {
                throw new KeyNotFoundException($"Status de serviço com ID {newStatusId} não existe.");
            }

            service.SetStatus(newStatus);
            await serviceRepository.SaveChangesAsync();

            return service.ToResponse();
        }
    }
}
