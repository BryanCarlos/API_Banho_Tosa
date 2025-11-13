namespace API_Banho_Tosa.Domain.Entities
{
    public class ServicePrice
    {
        public int AvailableServiceId { get; private set; }
        public AvailableService AvailableService { get; private set; } = null!;

        public int PetSizeId { get; private set; }
        public PetSize PetSize { get; private set; } = null!;

        public decimal Price { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
