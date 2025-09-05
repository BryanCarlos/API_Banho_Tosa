using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class BreedRepository : IBreedRepository
    {
        private readonly BanhoTosaContext _context;

        public BreedRepository(BanhoTosaContext context)
        {
            this._context = context;
        }

        public void DeleteBreed(Breed breed)
        {
            _context.Breeds.Remove(breed);
        }

        public async Task<Breed?> GetBreedByIdAsync(int id)
        {
            return await _context.Breeds
                .Include(b => b.AnimalType)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task InsertBreedAsync(Breed breed)
        {
            await _context.Breeds.AddAsync(breed);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Breed>> SearchBreedsAsync(SearchBreedRequest searchParams)
        {
            var query = _context.Breeds.AsNoTracking();

            if (searchParams.Id != null && searchParams.Id > 0)
            {
                query = query.Where(b => b.Id == searchParams.Id);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Name))
            {
                query = query.Where(b => EF.Functions.ILike(b.Name, $"{searchParams.Name}%"));
            }

            return await query.Include(b => b.AnimalType).ToListAsync();
        }

        public async Task<IReadOnlyCollection<Breed>> GetBreedsByAnimalTypeIdAsync(int animalTypeId)
        {
            return await _context.Breeds
                .Include(b => b.AnimalType)
                .Where(b => b.AnimalTypeId == animalTypeId)
                .ToListAsync();
        }
    }
}
