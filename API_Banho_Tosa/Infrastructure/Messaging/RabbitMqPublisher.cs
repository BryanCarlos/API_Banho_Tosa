using API_Banho_Tosa.Application.Common.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Text.Json;

namespace API_Banho_Tosa.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly ILogger<RabbitMqPublisher> _logger;
        private readonly IConnection _connection;
        private const string EXCHANGE_NAME = "topic_emails";
        private const string ROUTING_KEY = "email.confirmation";

        public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger, IConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        {
            var channelOpts = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);
            var properties = new BasicProperties
            {
                Persistent = true
            };

            using var channel = await _connection.CreateChannelAsync(channelOpts);

            await channel.ExchangeDeclareAsync(exchange: EXCHANGE_NAME, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null, cancellationToken: cancellationToken);

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            try
            {
                await channel.BasicPublishAsync(exchange: EXCHANGE_NAME, routingKey: ROUTING_KEY, mandatory: true, basicProperties: properties, body: body, cancellationToken: cancellationToken);

                _logger.LogInformation("Message sent successfully to the exchange {ExchangeName}!", EXCHANGE_NAME);
            }
            catch (PublishException ex)
            {
                if (ex.IsReturn)
                {
                    _logger.LogCritical(ex, "[RETURN] Message returned! Exchange couldn't find any queue binded with {RoutingKey}", ROUTING_KEY);
                }
                else
                {
                    _logger.LogCritical(ex, "[NACK] Broker denied the message:\n {MessageBody}", jsonMessage);
                }
            }
        }
    }
}
