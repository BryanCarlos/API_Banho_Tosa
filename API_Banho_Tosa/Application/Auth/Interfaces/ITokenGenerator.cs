using API_Banho_Tosa.Application.Auth.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Auth.Interfaces
{
    public interface ITokenGenerator
    {
        TokenResponse GenerateUserToken(User user);
    }
}
