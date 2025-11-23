using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class AvailableServiceRepository(BanhoTosaContext context) : IAvailableServiceRepository
    {
        public void AddAvailableService(AvailableService availableService)
        {
            context.AvailableServices.Add(availableService);
        }

        public async Task<bool> AvailableServiceExistAsync(string description)
        {
            return await context.AvailableServices.AnyAsync(serv => EF.Functions.ILike(serv.Description, description));
        }

        public async Task<AvailableService?> GetAvailableServiceByIdAsync(int id)
        {
            return await context.AvailableServices.FindAsync(id);
        }

        public async Task<AvailableService?> GetAvailableServiceByUuidAsync(Guid uuid)
        {
            return await context.AvailableServices.FirstOrDefaultAsync(serv => serv.Uuid == uuid);
        }

        public async Task<AvailableService?> GetDeletedAvailableServiceByUuidAsync(Guid uuid)
        {
            return await context.AvailableServices
                .IgnoreQueryFilters()
                .Where(s => s.DeletedAt != null)
                .FirstOrDefaultAsync(serv => serv.Uuid == uuid);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AvailableService>> SearchAvailableServicesAsync(string? description)
        {
            var query = context.AvailableServices.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(serv => EF.Functions.ILike(serv.Description, $"{description}%"));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<AvailableService>> SearchDeletedAvailableServicesAsync(string? description)
        {
            var query = context.AvailableServices
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(s => s.DeletedAt != null);

            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(serv => EF.Functions.ILike(serv.Description, $"{description}%"));
            }

            return await query.ToListAsync();
        }
    }
}
