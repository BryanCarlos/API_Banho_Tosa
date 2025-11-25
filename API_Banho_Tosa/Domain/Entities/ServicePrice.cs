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

        private ServicePrice() { }

        public static ServicePrice Create(int availableServiceId, int petSizeId, decimal servicePrice)
        {
            ValidatePrice(servicePrice);
            var now = DateTime.UtcNow;

            return new ServicePrice
            {
                AvailableServiceId = availableServiceId,
                PetSizeId = petSizeId,
                Price = servicePrice,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public void UpdateAvailableService(AvailableService availableService)
        {
            this.AvailableService = availableService;
            this.AvailableServiceId = availableService.Id;
        }

        public void UpdatePetSize(PetSize petSize)
        {
            this.PetSize = petSize;
            this.PetSizeId = petSize.Id;
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal price)
        {
            ValidatePrice(price);

            this.Price = price;
            this.MarkAsUpdated();
        }

        private static void ValidatePrice(decimal price)
        {
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be a value under zero.");
            }
        }
    }
}
