using API_Banho_Tosa.Application.Auth.DTOs;

namespace API_Banho_Tosa.Application.Auth.Services
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync(UserCreateRequest request);
        Task<TokenResponse> LoginAsync(UserLoginRequest request);
        Task<TokenResponse> RefreshTokensAsync(RefreshTokenRequest request);
        Task LogoutAsync(Guid userId);
    }
}
