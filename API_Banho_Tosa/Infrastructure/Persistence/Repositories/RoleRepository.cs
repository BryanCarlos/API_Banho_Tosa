using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private BanhoTosaContext _context;

        public RoleRepository(BanhoTosaContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByDescriptionAsync(string description)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Description == description);
        }
    }
}
