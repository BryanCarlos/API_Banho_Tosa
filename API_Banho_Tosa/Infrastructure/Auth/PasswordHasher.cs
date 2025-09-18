using API_Banho_Tosa.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API_Banho_Tosa.Infrastructure.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly IPasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

        public string Hash(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool Verify(string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
