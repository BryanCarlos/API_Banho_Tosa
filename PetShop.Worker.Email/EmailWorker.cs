using PetShop.Shared.Contracts.Events;
using PetShop.Worker.Email.EventHandlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PetShop.Worker.Email
{
    public class EmailWorker : BackgroundService
    {
        private const string MAIN_EXCHANGE_NAME = "topic_emails";
        private const string MAIN_QUEUE_NAME = "email_queue";
        private const string MAIN_ROUTING_KEY = "email.confirmation";

        private const string RETRY_EXCHANGE_NAME = "emails_retry_exchange";
        private const string RETRY_QUEUE_NAME = "email_queue_retry_30s";
        private const int RETRY_TTL_MS = 30 * 1000;
        private const int MAX_RETRIES = 3;

        private const string DEAD_EXCHANGE_NAME = "email_error_message_exchange";
        private const string DEAD_QUEUE_NAME = "email_error_queue";

        private readonly ILogger<EmailWorker> _logger;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;

        public EmailWorker(ILogger<EmailWorker> logger, IConnection connection, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _connection = connection;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = await _connection.CreateChannelAsync();

            var taskMainExchange = channel.ExchangeDeclareAsync(exchange: MAIN_EXCHANGE_NAME, type: ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: stoppingToken);
            var taskRetryExchange = channel.ExchangeDeclareAsync(exchange: RETRY_EXCHANGE_NAME, type: ExchangeType.Direct, durable: true, autoDelete: false, cancellationToken: stoppingToken);
            var taskDeadExchange = channel.ExchangeDeclareAsync(exchange: DEAD_EXCHANGE_NAME, type: ExchangeType.Direct, durable: true, autoDelete: false, cancellationToken: stoppingToken);

            await Task.WhenAll(taskMainExchange, taskRetryExchange, taskDeadExchange);

            var retryQueueArgs = new Dictionary<string, object?>
            {
                { "x-message-ttl", RETRY_TTL_MS },
                { "x-dead-letter-exchange", MAIN_EXCHANGE_NAME }
            };
            await channel.QueueDeclareAsync(queue: RETRY_QUEUE_NAME, durable: true, exclusive: false, autoDelete: false, arguments: retryQueueArgs, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(queue: RETRY_QUEUE_NAME, exchange: RETRY_EXCHANGE_NAME, routingKey: MAIN_ROUTING_KEY, cancellationToken: stoppingToken);

            var mainQueueArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", RETRY_EXCHANGE_NAME }
            };
            await channel.QueueDeclareAsync(queue: MAIN_QUEUE_NAME, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArgs, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(queue: MAIN_QUEUE_NAME, exchange: MAIN_EXCHANGE_NAME, routingKey: MAIN_ROUTING_KEY, cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(queue: DEAD_QUEUE_NAME, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(queue: DEAD_QUEUE_NAME, exchange: DEAD_EXCHANGE_NAME, routingKey: MAIN_ROUTING_KEY, cancellationToken: stoppingToken);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (sender, ev) => Consumer_ReceivedAsync(channel, ev, stoppingToken);

            await channel.BasicConsumeAsync(queue: MAIN_QUEUE_NAME, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            _logger.LogInformation("Worker started listening on queue {QueueName}", MAIN_QUEUE_NAME);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            _logger.LogInformation("Email worker stopping.");
        }

        private async Task Consumer_ReceivedAsync(IChannel channel, BasicDeliverEventArgs ev, CancellationToken cancellationToken)
        {
            var body = ev.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            long retryCount = 0;
            if (ev.BasicProperties.Headers?.ContainsKey("x-death") == true) 
            {
                var deathHeader = (List<object>)ev.BasicProperties.Headers["x-death"]!;
                var latestDeath = (Dictionary<string, object>)deathHeader[0];
                retryCount = (long)latestDeath["count"];
            }

            if (retryCount > MAX_RETRIES)
            {
                await channel.BasicPublishAsync(exchange: DEAD_EXCHANGE_NAME, routingKey: MAIN_ROUTING_KEY, body: body);
                _logger.LogWarning(
                    "Message exceeded the limit of {MaxRetriesCount} retry limit. Permanently discarding. Payload: {@MessageBody}", 
                    MAX_RETRIES, 
                    new { MessageBody = message }
                );

                await channel.BasicAckAsync(ev.DeliveryTag, multiple: false, cancellationToken);
                return;
            }

            try
            {
                var emailMessage = JsonSerializer.Deserialize<UserRegisteredEvent>(message);

                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<UserRegisteredEventHandler>();
                await handler.HandleAsync(emailMessage, cancellationToken);

                await channel.BasicAckAsync(ev.DeliveryTag, multiple: false, cancellationToken);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "An error occurred with the message received: {Message}", ex.Message);
                await channel.BasicNackAsync(deliveryTag: ev.DeliveryTag, multiple: false, requeue: false, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Email confirmation task have been canceled.");
                await channel.BasicNackAsync(deliveryTag: ev.DeliveryTag, multiple: false, requeue: false, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
                await channel.BasicNackAsync(deliveryTag: ev.DeliveryTag, multiple: false, requeue: false, cancellationToken);
            }
        }
    }
}
