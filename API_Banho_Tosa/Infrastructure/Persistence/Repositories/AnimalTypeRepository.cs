using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class AnimalTypeRepository : IAnimalTypeRepository
    {
        private readonly BanhoTosaContext _dbContext;

        public AnimalTypeRepository(BanhoTosaContext context)
        {
            this._dbContext = context;
        }

        public void DeleteAnimalType(AnimalType animalType)
        {
            _dbContext.AnimalTypes.Remove(animalType);
        }

        public async Task<IEnumerable<AnimalType>> SearchAnimalTypesAsync(SearchAnimalTypeRequest searchParams)
        {
            var query = _dbContext.AnimalTypes.AsNoTracking();

            if (searchParams.Id != null)
            {
                query = query.Where(at => at.Id == searchParams.Id);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Name))
            {
                query = query.Where(at => EF.Functions.ILike(at.Name, $"{searchParams.Name}%"));
            }

            return await query.ToListAsync();
        }

        public async Task<AnimalType?> GetAnimalTypeByIdAsync(int id)
        {
            return await _dbContext.AnimalTypes.FindAsync(id);
        }

        public async Task InsertAnimalTypeAsync(AnimalType animalType)
        {
            await _dbContext.AnimalTypes.AddAsync(animalType);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
