using API_Banho_Tosa.Domain.ValueObjects;

namespace API_Banho_Tosa.Application.Common.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(Email email) : base($"A user with the email '{email.Value}' is already registered.") { }
    }
}
