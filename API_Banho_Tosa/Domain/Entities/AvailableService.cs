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
    }
}
