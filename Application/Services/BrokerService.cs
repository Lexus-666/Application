using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using kursah_5semestr.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace kursah_5semestr.Services
{
    public class BrokerService : IBrokerService
    {
        private ConnectionFactory _connectionFactory;
        private IList<AsyncEventingBasicConsumer> _consumers = [];

        public BrokerService()
        {
            _connectionFactory = new ConnectionFactory {  HostName = "localhost" };
        }

        public async Task SendMessage(string exchange, object message)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);
            var json = JsonSerializer.Serialize(message);
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
            Console.WriteLine($"Subscribing to '{exchange}', queue name is '{queueName}'");
            consumer.ReceivedAsync += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message received '{message}'");
                await handler(message);
            };
            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
        }
    }
}
