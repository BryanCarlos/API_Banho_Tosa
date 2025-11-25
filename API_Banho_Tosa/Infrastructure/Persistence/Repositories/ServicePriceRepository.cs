using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class ServicePriceRepository(BanhoTosaContext context) : IServicePriceRepository
    {
        public void AddServicePrice(ServicePrice servicePrice)
        {
            context.ServicePrices.Add(servicePrice);
        }

        public void DeleteServicePrice(ServicePrice servicePrice)
        {
            context.ServicePrices.Remove(servicePrice);
        }

        public async Task<IEnumerable<ServicePrice>> GetAllByServiceIdAsync(int serviceId)
        {
            return await context.ServicePrices
                .Include(sp => sp.AvailableService)
                .Include(sp => sp.PetSize)
                .Where(sp => sp.AvailableServiceId == serviceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServicePrice>> GetAllByServiceUuidAsync(Guid serviceId)
        {
            return await context.ServicePrices
                .Include(sp => sp.AvailableService)
                .Include(sp => sp.PetSize)
                .Where(sp => sp.AvailableService.Uuid == serviceId)
                .ToListAsync();
        }

        public async Task<ServicePrice?> GetServicePriceByCompositeKeyAsync(int serviceId, int petSizeId)
        {
            return await context.ServicePrices
                .Include(sp => sp.AvailableService)
                .Include(sp => sp.PetSize)
                .FirstOrDefaultAsync(sp => sp.AvailableServiceId == serviceId && sp.PetSizeId == petSizeId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<bool> ServicePriceExistAsync(int serviceId, int petSizeId)
        {
            return await context.ServicePrices
                .AnyAsync(
                    sp => sp.AvailableServiceId == serviceId &&
                          sp.PetSizeId == petSizeId
                );
        }
    }
}
