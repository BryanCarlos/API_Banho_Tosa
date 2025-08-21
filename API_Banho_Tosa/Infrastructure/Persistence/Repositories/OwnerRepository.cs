using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private BanhoTosaContext _context;

        public OwnerRepository(BanhoTosaContext context)
        {
            _context = context;
        }

        public async Task DeleteOwnerAsync(Owner owner)
        {
            var ownerToDelete = await GetOwnerByIdAsync(owner.Id);

            if (ownerToDelete == null)
            {
                throw new KeyNotFoundException($"Owner with ID {owner.Id} not found.");
            }

            _context.Owners.Remove(ownerToDelete);
        }

        public async Task<Owner?> GetOwnerByIdAsync(int id)
        {
            return await _context.Owners.FindAsync(id);
        }

        public async Task<IEnumerable<Owner>> GetOwnersByNameAsync(string name)
        {
            return await _context.Owners.Where(o => EF.Functions.ILike(o.Name, $"%{name}%")).ToListAsync();
        }

        public async Task<IEnumerable<Owner>> GetOwnersByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return Enumerable.Empty<Owner>();
            }

            var cleanedPhoneToSearch = PhoneNumber.Clean(phone);

            if (cleanedPhoneToSearch == null)
            {
                return Enumerable.Empty<Owner>();
            }

            return await _context.Owners.Where(o => o.Phone != null && o.Phone.Value.StartsWith(cleanedPhoneToSearch)).ToListAsync();
        }

        public async Task<Owner?> GetOwnerByUuidAsync(Guid uuid)
        {
            return await _context.Owners.FirstOrDefaultAsync(o => o.Uuid == uuid);
        }

        public async Task<Owner?> GetOwnerByUuidIgnoringFiltersAsync(Guid uuid)
        {
            return await _context.Owners.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Uuid == uuid);
        }

        public async Task<IEnumerable<Owner>> GetOwnersAsync()
        {
            return await _context.Owners.IgnoreQueryFilters().ToListAsync();
        }

        public async Task<IEnumerable<Owner>> SearchOwnersAsync(SearchOwnerRequest searchParams)
        {
            var query = _context.Owners.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchParams.Name))
            {
                query = query.Where(o => EF.Functions.ILike(o.Name, $"%{searchParams.Name}%"));
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Phone))
            {
                PhoneNumber phoneNumberToSearch;

                try
                {
                    phoneNumberToSearch = PhoneNumber.Create(searchParams.Phone);
                }
                catch (ArgumentException)
                {
                    return Enumerable.Empty<Owner>();
                }
                
                query = query.Where(o => o.Phone == phoneNumberToSearch);
            }

            return await query.ToListAsync();
        }

        public async Task InsertOwnerAsync(Owner owner)
        {
            await _context.Owners.AddAsync(owner);
        }

        public async Task UpdateOwnerAsync(Owner owner)
        {
            var ownerToUpdate = await GetOwnerByIdAsync(owner.Id);

            if (ownerToUpdate == null)
            {
                throw new KeyNotFoundException($"Owner with ID {owner.Id} not found.");
            }

            ownerToUpdate.UpdateName(owner.Name);
            ownerToUpdate.UpdateAddress(owner.Address);

            var phoneString = owner.Phone?.Value;
            phoneString = PhoneNumber.Clean(phoneString);
            ownerToUpdate.UpdatePhone(phoneString);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Owner>> GetArchivedOwnersAsync()
        {
            return await _context.Owners
                .IgnoreQueryFilters()
                .Where(o => o.DeletedAt != null)
                .ToListAsync();
        }
    }
}
