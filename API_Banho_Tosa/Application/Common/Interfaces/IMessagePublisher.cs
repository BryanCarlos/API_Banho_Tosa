namespace API_Banho_Tosa.Application.Common.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
    }
}
