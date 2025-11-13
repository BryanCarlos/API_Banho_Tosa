namespace API_Banho_Tosa.Domain.Entities
{
    public class Service
    {
        public int Id { get; private set; }
        public Guid Uuid { get; private set; }
        public DateTime ServiceDate { get; private set; }

        public int PetId { get; private set; }
        public Pet Pet { get; private set; } = null!;

        public int ServiceStatusId { get; private set; }
        public ServiceStatus ServiceStatus { get; private set; } = null!;

        public int PaymentStatusId { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; } = null!;

        public DateTime? PaymentDate { get; private set; }
        public DateTime PaymentDueTime { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal AdditionalCharges { get; private set; }
        public decimal DiscountValue { get; private set; }
        public decimal FinalTotal { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public ICollection<ServiceItem> ServiceItems { get; private set; } = new List<ServiceItem>();
    }
}
