using API_Banho_Tosa.Application.Auth.DTOs;
using API_Banho_Tosa.Application.Auth.Mappers;
using API_Banho_Tosa.Domain.Interfaces;
using System;

namespace API_Banho_Tosa.Application.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserResponse> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("Attempted to get an user info with UUID {UserId} that was not found.", id);
                throw new KeyNotFoundException($"User with UUID {id} not found.");
            }

            return user.ToResponse();
        }
    }
}
