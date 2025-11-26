using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class PetRepository(BanhoTosaContext context) : IPetRepository
    {
        public void AddPet(Pet pet)
        {
            context.Pets.Add(pet);
        }

        public async Task<Pet?> GetPetByIdAsync(Guid id)
        {
            return await context.Pets
                .Include(p => p.Breed)
                    .ThenInclude(b => b.AnimalType)
                .Include(p => p.PetSize)
                .Include(p => p.Owners)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<Pet>> SearchPetsAsync(string? petName, string? ownerName, Guid? ownerId)
        {
            var query = context.Pets
                .AsNoTracking()
                .Include(p => p.Breed)
                    .ThenInclude(b => b.AnimalType)
                .Include(p => p.PetSize)
                .Include(p => p.Owners)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(petName))
            {
                query = query.Where(p => 
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(p.Name),
                        EF.Functions.Unaccent($"{petName}%")
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(ownerName))
            {
                query = query.Where(p => p.Owners.Any(o => 
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(o.Name),
                        EF.Functions.Unaccent($"{ownerName}%")
                    ))
                );
            }

            if (ownerId.HasValue)
            {
                query = query.Where(p => p.Owners.Any(o => o.Uuid == ownerId.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Pet>> SearchDeletedPetsAsync(string? petName, string? ownerName, Guid? ownerId)
        {
            var query = context.Pets
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(p => p.DeletedAt != null)
                .Include(p => p.Breed)
                    .ThenInclude(b => b.AnimalType)
                .Include(p => p.PetSize)
                .Include(p => p.Owners)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(petName))
            {
                query = query.Where(p => EF.Functions.ILike(p.Name, $"{petName}%"));
            }

            if (!string.IsNullOrWhiteSpace(ownerName))
            {
                query = query.Where(p => p.Owners.Any(o => EF.Functions.ILike(o.Name, $"{ownerName}%")));
            }

            if (ownerId.HasValue)
            {
                query = query.Where(p => p.Owners.Any(o => o.Uuid == ownerId.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<Pet?> GetDeletedPetByIdAsync(Guid id)
        {
            return await context.Pets
                .IgnoreQueryFilters()
                .Where(p => p.DeletedAt != null)
                .Include(p => p.Breed)
                    .ThenInclude(b => b.AnimalType)
                .Include(p => p.PetSize)
                .Include(p => p.Owners)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
