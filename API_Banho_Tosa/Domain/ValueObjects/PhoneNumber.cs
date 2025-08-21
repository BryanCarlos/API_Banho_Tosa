using System.ComponentModel.DataAnnotations;

namespace API_Banho_Tosa.Domain.ValueObjects
{
    public class PhoneNumber
    {
        [MinLength(10)]
        [MaxLength(25)]
        public string Value { get; private set; }

        private PhoneNumber(string value)
        {
            this.Value = value;
        }

        public static PhoneNumber Create(string? number)
        {
            var cleanedNumber = Clean(number);

            if (string.IsNullOrWhiteSpace(cleanedNumber))
            {
                throw new ArgumentNullException(nameof(number));
            }

            if (cleanedNumber.Length < 10 || cleanedNumber.Length > 25)
            {
                throw new ArgumentException("The phone number must have at leats 10 digits and a maximum of 25 digits.", nameof(number));
            }

            return new PhoneNumber(cleanedNumber);
        }

        public override string ToString()
        {
            return Value;
        }

        public static string? Clean(string? number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return null;
            }

            return new string(number.Where(char.IsDigit).ToArray()); ;
        }
    }
}
