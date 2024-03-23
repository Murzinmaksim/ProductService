using System.Text;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using InventoryService.Data;
using Newtonsoft.Json;
using InventoryService.Services.Interface;

namespace InventoryService.Services
{
    public class RabbitMQListenerService : BackgroundService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQListenerService(IConnectionFactory factory, IServiceScopeFactory serviceScopeFactory)
        {
            _connectionFactory = factory;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            await Task.Yield();

            var queueName = "CRUD-messages";

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (_, args) =>
            {
                await ProcessMessageAsync(args.Body.ToArray(), args.RoutingKey, _serviceScopeFactory.CreateScope());
                // Подтверждение успешного получения сообщения
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume(consumer, queueName);
        }

        private async Task ProcessMessageAsync(byte[] body, string routingKey, IServiceScope scope)
        {
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received message: {0}", message);

            using (var serviceScope = scope)
            {
                var serviceProvider = serviceScope.ServiceProvider;
                var context = serviceProvider.GetRequiredService<InventoryServiceContext>();

                switch (routingKey)
                {
                    case "inventory.product.put":
                        await ProcessPutRequestAsync(message, context, serviceProvider);
                        break;
                    case "inventory.product.post":
                        await ProcessPostRequestAsync(message, context, serviceProvider);
                        break;
                    case "inventory.product.delete":
                        await ProcessDeleteRequestAsync(message, context, serviceProvider);
                        break;
                    default:
                        Console.WriteLine("Received message with unexpected routing key: {0}", routingKey);
                        break;
                }
            }
        }

        private async Task ProcessPutRequestAsync(string message, InventoryServiceContext context, IServiceProvider serviceProvider)
        {
            var updateService = serviceProvider.GetRequiredService<IUpdateService>();
            var inventoryUpdate = JsonConvert.DeserializeObject<Inventory>(message);

            try
            {
                await updateService.UpdateInventoryAsync(inventoryUpdate);
                Console.WriteLine("Inventory item updated successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Invalid inventory data: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Inventory item not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating inventory: {ex.Message}");
            }
        }

        private async Task ProcessPostRequestAsync(string message, InventoryServiceContext context, IServiceProvider serviceProvider)
        {
            var createService = serviceProvider.GetRequiredService<ICreateService>();
            var newInventory = JsonConvert.DeserializeObject<Inventory>(message);

            try
            {
                await createService.CreateInventoryAsync(newInventory);
                Console.WriteLine("New inventory item created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating inventory: {ex.Message}");
            }
        }

        private async Task ProcessDeleteRequestAsync(string message, InventoryServiceContext context, IServiceProvider serviceProvider)
        {
            var deleteService = serviceProvider.GetRequiredService<IDeleteService>();
            var inventoryToDelete = JsonConvert.DeserializeObject<Inventory>(message);

            try
            {
                await deleteService.DeleteInventoryAsync(inventoryToDelete.id);
                Console.WriteLine("Inventory item deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Inventory item not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting inventory: {ex.Message}");
            }
        }
    }
}
