namespace API_Banho_Tosa.Domain.Entities
{
    public class ServiceStatus
    {
        public int Id { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public ICollection<Service> Services { get; private set; } = new List<Service>();
    }
}
