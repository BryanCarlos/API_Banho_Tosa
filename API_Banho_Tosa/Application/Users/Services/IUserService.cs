using API_Banho_Tosa.Application.Auth.DTOs;

namespace API_Banho_Tosa.Application.Users.Services
{
    public interface IUserService
    {
        Task<UserResponse> GetUserByIdAsync(Guid id);
    }
}
