namespace ProductService.Services.Interface
{
    public interface IRabbitMQService
    {
        void SendMessage(string queueName, string message);
    }
}
