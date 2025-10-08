using API_Banho_Tosa.Application.PetSizes.DTOs;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class PetSizeRepository : IPetSizeRepository
    {
        private readonly BanhoTosaContext _context;

        public PetSizeRepository(BanhoTosaContext context)
        {
            _context = context;
        }

        public void DeletePetSize(PetSize petSize)
        {
            _context.PetSizes.Remove(petSize);
        }

        public Task<PetSize?> GetPetSizeByDescriptionAsync(string description)
        {
            return _context.PetSizes.FirstOrDefaultAsync(p => EF.Functions.ILike(p.Description, description));
        }

        public async Task<PetSize?> GetPetSizeByIdAsync(int id)
        {
            return await _context.PetSizes.FindAsync(id);
        }

        public void InsertPetSize(PetSize petSize)
        {
            _context.PetSizes.Add(petSize);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PetSize>> SearchPetSizesAsync(SearchPetSizeRequest searchParams)
        {
            var query = _context.PetSizes.AsNoTracking();

            if (searchParams.Id != null)
            {
                query = query.Where(p => p.Id == searchParams.Id);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Description))
            {
                query = query.Where(p => EF.Functions.ILike(p.Description, $"%{searchParams.Description}%"));
            }

            return await query.ToListAsync();
        }
    }
}
