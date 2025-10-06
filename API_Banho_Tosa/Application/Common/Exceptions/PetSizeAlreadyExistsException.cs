using API_Banho_Tosa.Domain.ValueObjects;

namespace API_Banho_Tosa.Application.Common.Exceptions
{
    public class PetSizeAlreadyExistsException : Exception
    {
        public PetSizeAlreadyExistsException(string description) : base($"A pet size with the description '{description}' is already registered.") { }
    }
}
