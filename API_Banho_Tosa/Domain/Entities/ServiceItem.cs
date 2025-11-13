namespace API_Banho_Tosa.Domain.Entities
{
    public class ServiceItem
    {
        public int Id { get; private set; }

        public int ServiceId { get; private set; }
        public Service Service { get; private set; } = null!;

        public int AvailableServiceId { get; private set; }
        public AvailableService AvailableService { get; private set; } = null!;

        public decimal PriceAtTheTime { get; private set; }
        public int Quantity { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
