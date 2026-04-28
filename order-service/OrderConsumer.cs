using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using order_service.Context;
using order_service.Entities;
using order_service.Events;
using System.Text.Json;

namespace order_service
{
    public class OrderConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public OrderConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "order-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consumer = new ConsumerBuilder<string, string>(config).Build();

            consumer.Subscribe(new[]
            {
                "payment-approved",
                "payment-failed"
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    using var scope = _scopeFactory.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                    var topic = result.Topic;
                    var key = result.Message.Key;

                    if (topic == "payment-approved")
                    {
                        var evt = JsonSerializer.Deserialize<PaymentApprovedEvent>(result.Message.Value);

                        if (evt == null)
                        {
                            Console.WriteLine("[Order] Error: PaymentApprovedEvent is null");
                            consumer.Commit(result);
                            continue;
                        }

                        var exists = await context.Orders.AnyAsync(p => p.Id == evt.OrderId);

                        if (exists)
                        {
                            Console.WriteLine($"[Payment] Already processed: {evt.OrderId}");
                            consumer.Commit(result);
                            continue;
                        }

                        var order = context.Orders.FirstOrDefault(o => o.Id == evt.OrderId);

                        if (order != null)
                        {
                            order.Status = OrderStatus.Paid;
                            context.Orders.Update(order);
                            await context.SaveChangesAsync();
                        }

                        Console.WriteLine($"[Order] Payment APPROVED → Order {evt.OrderId} = PAID");
                        consumer.Commit(result);
                    }

                    if (topic == "payment-failed")
                    {
                        var evt = JsonSerializer.Deserialize<PaymentFailedEvent>(result.Message.Value);

                        if (evt == null)
                        {
                            Console.WriteLine("[Order] Error: PaymentApprovedEvent is null");
                            consumer.Commit(result);
                            continue;
                        }

                        var exists = await context.Orders.AnyAsync(p => p.Id == evt.OrderId);

                        if (exists)
                        {
                            Console.WriteLine($"[Payment] Already processed: {evt.OrderId}");
                            consumer.Commit(result);
                            continue;
                        }

                        var order = context.Orders.FirstOrDefault(o => o.Id == evt.OrderId);

                        if (order != null)
                        {
                            order.Status = OrderStatus.Failed;
                            context.Orders.Update(order);
                            await context.SaveChangesAsync();
                        }

                        Console.WriteLine($"[Order] Payment FAILED → Order {evt.OrderId} = FAILED");
                        consumer.Commit(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Order] Error: {ex.Message}");
                }
            }
        }
    }
}
