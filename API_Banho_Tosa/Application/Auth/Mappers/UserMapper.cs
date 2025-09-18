using API_Banho_Tosa.Application.Auth.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Auth.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(this User user)
        {
            return new UserResponse(user.Id, user.CreatedAt, user.Username, user.Email.Value);
        }
    }
}
