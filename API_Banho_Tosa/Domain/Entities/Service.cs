using API_Banho_Tosa.Domain.Enums;
using Datadog.Trace.Ci;

namespace API_Banho_Tosa.Domain.Entities
{
    public class Service
    {
        public int Id { get; private set; }
        public Guid Uuid { get; private set; }
        public DateTime ServiceDate { get; private set; }

        public Guid PetUuid { get; private set; }
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

        private Service() { }

        public static Service Create(
            Guid petId,
            DateTime serviceDate,
            IEnumerable<ServiceItem> items,
            decimal discount = 0,
            decimal additionalCharges = 0)
        {
            var now = DateTime.UtcNow;

            var service = new Service
            {
                Uuid = Guid.CreateVersion7(),
                PetUuid = petId,
                ServiceDate = serviceDate,
                PaymentDueTime = serviceDate.AddDays(7),
                ServiceStatusId = (int)ServiceStatusEnum.Agendado,
                PaymentStatusId = (int)PaymentStatusEnum.Pendente,
                CreatedAt = now,
                UpdatedAt = now,
                Subtotal = 0
            };

            foreach (var item in items)
            {
                service.ServiceItems.Add(item);
                service.Subtotal += item.TotalItem;
            }

            service.SetFinancials(discount, additionalCharges);

            service.CalculateTotal();

            return service;
        }

        public void CalculateTotal()
        {
            this.FinalTotal = this.Subtotal + this.AdditionalCharges - this.DiscountValue;
        }

        public void SetFinancials(decimal discount, decimal additionalCharges)
        {
            if (discount < 0)
            {
                throw new ArgumentException("Discount cannot be a value lower than zero.");
            }
            if (additionalCharges < 0)
            {
                throw new ArgumentException("Additional charges cannot be a value lower than zero.");
            }

            this.DiscountValue = discount;
            this.AdditionalCharges = additionalCharges;
        }

        public void SetStatus(ServiceStatus status)
        {
            this.ServiceStatusId = status.Id;
            this.ServiceStatus = status;
            this.MarkAsUpdated();
        }

        public void SetPaymentStatus(PaymentStatus paymentStatus)
        {
            this.PaymentStatusId = paymentStatus.Id;
            this.PaymentStatus = paymentStatus;
            this.MarkAsUpdated();
        }

        public void SetPet(Pet pet)
        {
            this.Pet = pet;
        }

        public void Delete()
        {
            if (this.IsDeleted())
            {
                throw new InvalidOperationException("Service already deleted.");
            }

            this.DeletedAt = DateTime.UtcNow;
            this.MarkAsUpdated();
        }

        public void Reactivate()
        {
            if (!this.IsDeleted())
            {
                throw new InvalidOperationException("Service already active.");
            }

            this.DeletedAt = null;
            this.MarkAsUpdated();
        }

        public void SetPaymentDate(DateTime? date)
        {
            this.PaymentDate = date;
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        private bool IsDeleted()
        {
            return this.DeletedAt.HasValue;
        }

        public void Update(DateTime serviceDate, List<ServiceItem> serviceItems, decimal? discountValue = 0, decimal? additionalCharges = 0)
        {
            this.ServiceItems.Clear();

            this.Subtotal = 0;
            this.FinalTotal = 0;

            this.ServiceDate = serviceDate;
            this.PaymentDueTime = serviceDate.AddDays(7);

            foreach (var item in serviceItems)
            {
                this.ServiceItems.Add(item);
                this.Subtotal += item.TotalItem;
            }

            this.SetFinancials(discountValue ?? 0, additionalCharges ?? 0);

            this.CalculateTotal();
            this.MarkAsUpdated();
        }
    }
}
