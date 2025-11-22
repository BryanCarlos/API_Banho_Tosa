namespace API_Banho_Tosa.Domain.Entities
{
    public class PaymentStatus
    {
        public int Id { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public ICollection<Service> Services { get; private set; } = new List<Service>();

        private PaymentStatus() { }

        public static PaymentStatus Create(string description)
        {
            return new PaymentStatus { Description = ValidateDescription(description) };
        }

        public void UpdateDescription(string description)
        {
            this.Description = ValidateDescription(description);
        }

        private static string ValidateDescription(string description)
        {
            var treatedDescription = description?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Payment status description cannot be empty.");
            }

            return treatedDescription;
        }
    }
}
