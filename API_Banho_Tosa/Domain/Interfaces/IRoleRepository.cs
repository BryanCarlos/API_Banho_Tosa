using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByDescriptionAsync(string description);
    }
}
