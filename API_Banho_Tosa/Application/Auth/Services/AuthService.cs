using API_Banho_Tosa.Application.Auth.DTOs;
using API_Banho_Tosa.Application.Auth.Interfaces;
using API_Banho_Tosa.Application.Auth.Mappers;
using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Domain.Constants;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Domain.ValueObjects;
using PetShop.Shared.Contracts.Events;
using System;

namespace API_Banho_Tosa.Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator, IMessagePublisher messagePublisher, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<TokenResponse> LoginAsync(UserLoginRequest request)
        {
            var email = Email.Create(request.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
            {
                _logger.LogWarning(
                    "Attempted to login user with email {UserEmail} but it doesn't exist or the password is wrong.",
                    request.Email
                );

                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!user.IsEmailConfirmed)
            {
                _logger.LogWarning(
                    "User {UserEmail} tried to login but has not confirmed their email yet.",
                    user.Email.ToString()
                );

                throw new UnauthorizedAccessException("Email address not confirmed.");
            }

            return await CreateAndReturnUserToken(user);
        }

        public async Task<TokenResponse> RefreshTokensAsync(RefreshTokenRequest request)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken);

            if (user == null || !user.IsRefreshTokenValid())
            {
                _logger.LogWarning(
                    "An invalid or expired refresh token was used. Token start with: {token}",
                    request.RefreshToken.Substring(0, Math.Min(request.RefreshToken.Length, 8))
                );
                throw new UnauthorizedAccessException("The refresh token has already expired.");
            }

            return await CreateAndReturnUserToken(user);
        }

        public async Task<UserResponse> RegisterAsync(UserCreateRequest request)
        {
            _logger.LogInformation(
                "Attempting to register a new user with email {UserEmail}",
                request.Email
            );

            var userEmail = Email.Create(request.Email);
            var userEmailExists = await _userRepository.GetUserByEmailAsync(userEmail);

            if (userEmailExists != null)
            {
                throw new UserAlreadyExistsException(userEmail);
            }

            var hashedPassword = _passwordHasher.Hash(request.Password);
            var user = User.Create(userEmail, request.Name, hashedPassword);

            var defaultRole = await _roleRepository.GetByDescriptionAsync(AppRoles.User);

            if (defaultRole == null)
            {
                _logger.LogCritical(
                    "CRITICAL FAILURE: The default role {DefaultRole} was not found in the database. User registration is blocked.",
                    AppRoles.User
                );

                throw new ConfigurationException("An unexpected error occurred while processing the request. The support team has been notified.");
            }

            user.Roles.Add(defaultRole);

            _userRepository.InsertUser(user);
            await _userRepository.SaveChangesAsync();

            var userRegisteredEvent = new UserRegisteredEvent(userEmail.Value, user.EmailConfirmationToken!, user.EmailTokenExpiryDate!.Value);
            await _messagePublisher.PublishAsync(userRegisteredEvent);

            return user.ToResponse();
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("Attempted to logout an user with ID {UserId} that was not found.", userId);
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            user.ClearRefreshToken();
            await _userRepository.SaveChangesAsync();
        }

        private async Task<TokenResponse> CreateAndReturnUserToken(User user)
        {
            user.UpdateLastLogin();

            var tokenResponse = _tokenGenerator.GenerateUserToken(user);

            user.UpdateRefreshToken(tokenResponse.RefreshToken);
            await _userRepository.SaveChangesAsync();

            return tokenResponse;
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            var user = await _userRepository.GetUserByConfirmationTokenAsync(token);

            if (user == null)
            {
                _logger.LogWarning("Attempted to confirm an user with confirmation token that starts with {ConfirmationToken} and was not able to find any user.", token.ToString().Substring(0, 10));
                //throw new KeyNotFoundException($"User with token {token} not found");
                return false;
            }

            if (user.IsEmailConfirmed)
            {
                //throw new InvalidOperationException("User already confirmed his email.");
                _logger.LogWarning("Attempted to confirm an user with email {UserEmail} that has already confirmed his email.", user.Email.ToString());
                return false;
            }

            if (DateTime.UtcNow >= user.EmailTokenExpiryDate)
            {
                //throw new InvalidOperationException("The token has expired.");
                _logger.LogWarning("User with email {UserEmail} tried to use an expired token to confirm his email.", user.Email.ToString());
                return false;
            }

            user.ConfirmEmail();
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("{UserName} confirmed his email successfully.", user.Username);

            return true;
        }
    }
}
