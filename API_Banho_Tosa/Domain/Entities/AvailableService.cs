namespace API_Banho_Tosa.Domain.Entities
{
    public class AvailableService
    {
        public int Id { get; private set; }
        public Guid Uuid { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public int? ServiceDurationMinutes { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public ICollection<ServicePrice> ServicePrices { get; private set; } = new List<ServicePrice>();
        public ICollection<ServiceItem> ServiceItems { get; private set; } = new List<ServiceItem>();

        private AvailableService() { }

        public static AvailableService Create(string description, int? durationInMinutes)
        {
            var treatedDescription = ValidateDescription(description);
            var uuid = Guid.CreateVersion7();

            var now = DateTime.UtcNow;

            return new AvailableService
            {
                Uuid = uuid,
                Description = treatedDescription,
                ServiceDurationMinutes = durationInMinutes,
                CreatedAt = now,
                UpdatedAt = now
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

        public void Delete()
        {
            if (this.IsDeleted())
            {
                throw new InvalidOperationException("Available service already deleted.");
            }

            this.DeletedAt = DateTime.UtcNow;
            this.MarkAsUpdated();
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        internal void UpdateDescription(string description)
        {
            var treatedDescription = ValidateDescription(description);

            this.Description = treatedDescription;
            this.MarkAsUpdated();
        }

        internal void UpdateServiceMinutes(int? durationInMinutes)
        {
            this.ServiceDurationMinutes = durationInMinutes;
            this.MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (!this.IsDeleted())
            {
                throw new InvalidOperationException("Available service already active.");
            }

            this.DeletedAt = null;
            this.MarkAsUpdated();
        }

        public bool IsDeleted()
        {
            return this.DeletedAt.HasValue;
        }
    }
}
