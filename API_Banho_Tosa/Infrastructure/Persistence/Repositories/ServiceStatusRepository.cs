using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class ServiceStatusRepository(BanhoTosaContext context) : IServiceStatusRepository
    {
        public void AddServiceStatus(ServiceStatus serviceStatus)
        {
            context.ServiceStatuses.Add(serviceStatus);
        }

        public void DeleteServiceStatus(ServiceStatus serviceStatus)
        {
            context.ServiceStatuses.Remove(serviceStatus);
        }

        public async Task<IEnumerable<ServiceStatus>> SearchServiceStatusesAsync(string? description)
        {
            var query = context.ServiceStatuses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(ss => EF.Functions.ILike(ss.Description, $"{description}%"));
            }

            return await query.ToListAsync();
        }

        public async Task<ServiceStatus?> GetServiceStatusByIdAsync(int id)
        {
            return await context.ServiceStatuses.FindAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<bool> ServiceStatusExistAsync(string description)
        {
            return await context.ServiceStatuses.AnyAsync(ss => EF.Functions.ILike(ss.Description, description));
        }
    }
}
