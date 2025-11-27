using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class ServiceRepository(BanhoTosaContext context) : IServiceRepository
    {
        public void AddService(Service service)
        {
            context.Services.Add(service);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<Service?> GetServiceByUuidAsync(Guid id)
        {
            return await GetBaseQuery()
                .FirstOrDefaultAsync(s => s.Uuid == id);
        }

        public async Task<Service?> GetDeletedServiceByUuidAsync(Guid id)
        {
            return await GetBaseQuery()
                .IgnoreQueryFilters()
                .Where(s => s.DeletedAt != null)
                .FirstOrDefaultAsync(s => s.Uuid == id);
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(DateTime? startDate, DateTime? endDate, int? statusId, int? paymentStatusId, Guid? petId, Guid? ownerId)
        {
            var query = GetBaseQuery().AsNoTracking();

            if (startDate.HasValue)
            {
                query = query.Where(s => s.ServiceDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(s => s.ServiceDate < endDate.Value);
            }
            if (statusId.HasValue)
            {
                query = query.Where(s => s.ServiceStatusId == statusId.Value);
            }
            if (paymentStatusId.HasValue)
            {
                query = query.Where(s => s.PaymentStatusId == paymentStatusId.Value);
            }
            if (petId.HasValue)
            {
                query = query.Where(s => s.PetUuid == petId.Value);
            }
            if (ownerId.HasValue)
            {
                query = query.Where(s => s.Pet.Owners.Any(o => o.Uuid == ownerId.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Service>> SearchDeletedServicesAsync(DateTime? startDate, DateTime? endDate, int? statusId, int? paymentStatusId, Guid? petId, Guid? ownerId)
        {
            var query = GetBaseQuery()
                .IgnoreQueryFilters()
                .Where(s => s.DeletedAt != null)
                .AsNoTracking();

            if (startDate.HasValue)
            {
                query = query.Where(s => s.ServiceDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(s => s.ServiceDate < endDate.Value);
            }
            if (statusId.HasValue)
            {
                query = query.Where(s => s.ServiceStatusId == statusId.Value);
            }
            if (paymentStatusId.HasValue)
            {
                query = query.Where(s => s.PaymentStatusId == paymentStatusId.Value);
            }
            if (petId.HasValue)
            {
                query = query.Where(s => s.PetUuid == petId.Value);
            }
            if (ownerId.HasValue)
            {
                query = query.Where(s => s.Pet.Owners.Any(o => o.Uuid == ownerId.Value));
            }

            return await query.ToListAsync();
        }

        private IQueryable<Service> GetBaseQuery()
        {
            return context.Services
                .AsSplitQuery()
                .Include(s => s.Pet)
                    .ThenInclude(p => p.Breed)
                        .ThenInclude(b => b.AnimalType)
                .Include(s => s.Pet)
                    .ThenInclude(p => p.PetSize)
                .Include(s => s.Pet)
                    .ThenInclude(p => p.Owners)
                .Include(s => s.ServiceStatus)
                .Include(s => s.PaymentStatus)
                .Include(s => s.ServiceItems)
                    .ThenInclude(si => si.AvailableService);
        }

        public void DeleteServiceItems(ICollection<ServiceItem> serviceItems)
        {
            context.ServiceItems.RemoveRange(serviceItems);
        }
    }
}
