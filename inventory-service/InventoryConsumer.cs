using Confluent.Kafka;
using inventory_service.Context;
using inventory_service.Entities;
using inventory_service.Events;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace inventory_service
{
    public class InventoryConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public InventoryConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "inventory-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(new[]
            {
                "product-created",
                "payment-approved"
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

                    if (result.Topic == "product-created")
                    {
                        await HandleProductCreatedAsync(context, result.Message.Value, stoppingToken);
                        consumer.Commit(result);
                        continue;
                    }

                    if (result.Topic == "payment-approved")
                    {
                        await HandlePaymentApprovedAsync(context, result.Message.Value, stoppingToken);
                        consumer.Commit(result);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Inventory] Error consuming message: {ex.Message}");
                }
            }
        }

        private static async Task HandleProductCreatedAsync(
            InventoryDbContext context,
            string message,
            CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<ProductCreatedEvent>(message);

            if (evt == null)
            {
                Console.WriteLine("[Inventory] Invalid ProductCreatedEvent");
                return;
            }

            var exists = await context.InventoryItems
                .AnyAsync(item => item.ProductId == evt.ProductId, cancellationToken);

            if (exists)
            {
                Console.WriteLine($"[Inventory] Product already has stock: {evt.ProductId}");
                return;
            }

            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = evt.ProductId,
                Quantity = evt.InitialQuantity,
                CreatedAt = DateTime.UtcNow
            };

            context.InventoryItems.Add(inventoryItem);
            await context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"[Inventory] Stock created for product {evt.ProductId}: {evt.InitialQuantity}");
        }

        private static async Task HandlePaymentApprovedAsync(
            InventoryDbContext context,
            string message,
            CancellationToken cancellationToken)
        {
            var evt = JsonSerializer.Deserialize<PaymentApprovedEvent>(message);

            if (evt == null)
            {
                Console.WriteLine("[Inventory] Invalid PaymentApprovedEvent");
                return;
            }

            var inventoryItem = await context.InventoryItems
                .FirstOrDefaultAsync(item => item.ProductId == evt.ProductId, cancellationToken);

            if (inventoryItem == null)
            {
                Console.WriteLine($"[Inventory] Stock not found for product: {evt.ProductId}");
                return;
            }

            if (inventoryItem.Quantity < evt.Quantity)
            {
                Console.WriteLine($"[Inventory] Insufficient stock for product {evt.ProductId}. Current: {inventoryItem.Quantity}, requested: {evt.Quantity}");
                return;
            }

            inventoryItem.Quantity -= evt.Quantity;
            await context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"[Inventory] Payment approved. Product {evt.ProductId} stock reduced by {evt.Quantity}. Current: {inventoryItem.Quantity}");
        }
    }
}
