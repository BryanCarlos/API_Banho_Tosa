using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace API_Banho_Tosa.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private BanhoTosaContext _context;

        public UserRepository(BanhoTosaContext context)
        {
            _context = context;
        }

        public Task<User?> GetUserByConfirmationTokenAsync(string token)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
        }

        public async Task<User?> GetUserByEmailAsync(Email email)
        {
            return await _context.Users
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public void InsertUser(User user)
        {
            _context.Users.AddAsync(user);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
