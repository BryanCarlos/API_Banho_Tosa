using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace API_Banho_Tosa.Domain.ValueObjects
{
    public class Email
    {
        public string Value { get; private set; }

        private Email(string email)
        {
            this.Value = email;
        }

        public static Email Create(string? email)
        {
            if (!IsValid(email))
            {
                throw new FormatException("The email format is not valid.");
            }

            return new Email(email!.Trim().ToLowerInvariant());
        }

        private static bool IsValid(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            try
            {
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }
    }
}
