using API_Banho_Tosa.Application.Auth.DTOs;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.ValueObjects;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(Email email);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        void InsertUser(User user);
        Task<int> SaveChangesAsync();
    }
}
