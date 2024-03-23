using InventoryService.Data;
using InventoryService.Services;
using InventoryService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("InventoryServiceConnection");

builder.Services.AddDbContext<InventoryServiceContext>(options =>
    options.UseNpgsql(connectionString));

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
builder.Services.AddHostedService<RabbitMQListenerService>();
builder.Services.AddScoped<IUpdateService, UpdateService>();
builder.Services.AddScoped<ICreateService, CreateService>();
builder.Services.AddScoped<IDeleteService, DeleteService>();

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
