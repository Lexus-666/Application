using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using kursah_5semestr.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace kursah_5semestr.Services
{
    public class BrokerService : IBrokerService
    {
        private ConnectionFactory _connectionFactory;
        private IList<AsyncEventingBasicConsumer> _consumers = [];
        private ILogger _logger;

        public BrokerService(ILogger<BrokerService> logger)
        {
            _connectionFactory = new ConnectionFactory {  HostName = "localhost" };
            _logger = logger;
        }

        public async Task SendMessage(string exchange, object message)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);
            JsonSerializerOptions options = new JsonSerializerOptions();
            var enumConverter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
            options.Converters.Add(enumConverter);
            var json = JsonSerializer.Serialize(message, options);
            _logger.LogInformation($"Sending message: {message}");
            var body = Encoding.UTF8.GetBytes(json);
            await channel.BasicPublishAsync(exchange: exchange, routingKey: "", body: body);
        }

        public async Task Subscribe(string exchange, Func<string, Task> handler)
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);
            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            string queueName = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(queue: queueName, exchange: exchange, routingKey: string.Empty);
            var consumer = new AsyncEventingBasicConsumer(channel);
            _consumers.Add(consumer);
            _logger.LogInformation($"Subscribing to '{exchange}', queue name is '{queueName}'");
            consumer.ReceivedAsync += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Message received '{message}'");
                await handler(message);
            };
            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
        }
    }
}
