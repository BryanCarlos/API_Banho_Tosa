
namespace API_Banho_Tosa.Domain.Entities
{
    public class ServiceStatus
    {
        public int Id { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public ICollection<Service> Services { get; private set; } = new List<Service>();

        private ServiceStatus() { }

        public static ServiceStatus Create(string description)
        {
            var treatedDescription = ValidateDescription(description);

            return new ServiceStatus
            {
                Description = treatedDescription
            };
        }

        private static string ValidateDescription(string description)
        {
            var treatedDescription = description?.Trim();

            if (string.IsNullOrWhiteSpace(treatedDescription))
            {
                throw new ArgumentException("Description cannot be empty.");
            }

            return treatedDescription.ToUpper();
        }

        public void UpdateDescription(string description)
        {
            var treatedDescription = ValidateDescription(description);

            this.Description = treatedDescription;
        }
    }
}
