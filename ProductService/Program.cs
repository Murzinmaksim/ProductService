using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Services;
using ProductService.Services.Interface;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ProductServiceConnection");

builder.Services.AddDbContext<ProductServiceContext>(options =>
    options.UseNpgsql(connectionString));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register RabbitMQ ConnectionFactory
builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory()
{
    Endpoint = new AmqpTcpEndpoint(),
    DispatchConsumersAsync = true
});
// Register RabbitMQListenerService
builder.Services.AddScoped<IRabbitMQService, RabbitMQSenderService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
