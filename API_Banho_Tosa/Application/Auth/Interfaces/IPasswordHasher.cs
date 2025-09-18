using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Auth.Interfaces
{
    public interface IPasswordHasher
    {
        bool Verify(string hashedPassword, string password);
        string Hash(string password);
    }
}
