using ProductService.Services.Interface;
using RabbitMQ.Client;
using System.Text;

namespace ProductService.Services
{
    public class RabbitMQSenderService : IRabbitMQService
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQSenderService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void SendMessage(string message, string routingKey)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "inventory.events", routingKey: routingKey, basicProperties: null, body: body);

            }
        }
    }
}
