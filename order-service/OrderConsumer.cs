using Confluent.Kafka;
using order_service.Context;
using order_service.Entities;
using order_service.Events;
using System.Text.Json;

namespace order_service
{
    public class OrderConsumer : BackgroundService
    {
        private readonly OrderDbContext _context;
        public OrderConsumer(OrderDbContext context)
        {
            _context = context;
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

                    var topic = result.Topic;
                    var key = result.Message.Key;

                    if (topic == "payment-approved")
                    {
                        var evt = JsonSerializer.Deserialize<PaymentApprovedEvent>(result.Message.Value);

                        var order = _context.Orders.FirstOrDefault(o => o.Id == evt.OrderId);
                        if (order != null)
                        {
                            order.Status = OrderStatus.Paid;
                            _context.Orders.Update(order);
                            await _context.SaveChangesAsync();
                        }

                        Console.WriteLine($"[Order] Payment APPROVED → Order {evt.OrderId} = PAID");
                    }

                    if (topic == "payment-failed")
                    {
                        var evt = JsonSerializer.Deserialize<PaymentFailedEvent>(result.Message.Value);

                        var order = _context.Orders.FirstOrDefault(o => o.Id == evt.OrderId);

                        if (order != null)
                        {
                            order.Status = OrderStatus.Failed;
                            _context.Orders.Update(order);
                            await _context.SaveChangesAsync();
                        }

                        Console.WriteLine($"[Order] Payment FAILED → Order {evt.OrderId} = FAILED");
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
