using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using payment_service.Context;
using payment_service.Entities;
using payment_service.Events;
using System.Text.Json;

namespace payment_service
{
    //TODO criar serviço de criação de pagamento 
    public class PaymentConsumer : BackgroundService
    {
        private readonly KafkaProducer _producer;
        private readonly IServiceScopeFactory _scopeFactory;
        public PaymentConsumer(KafkaProducer producer, IServiceScopeFactory scopeFactory)
        {
            _producer = producer;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "payment-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("order-created");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Message.Value);

                    if (orderEvent == null)
                    {
                        Console.WriteLine("[Payment] Invalid message");
                        consumer.Commit(result);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

                    var exists = await context.Payments.AnyAsync(p => p.OrderId == orderEvent.OrderId);

                    if (exists)
                    {
                        Console.WriteLine($"[Payment] Already processed: {orderEvent.OrderId}");
                        consumer.Commit(result);
                        continue;
                    }

                    var payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderEvent.OrderId,
                        Status = PaymentStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Payments.Add(payment);
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[Payment] Processing Order: {orderEvent.OrderId}");

                    var success = Random.Shared.Next(0, 2) == 1;

                    if (success)
                    {
                        payment.Status = PaymentStatus.Approved;
                        await _producer.ProduceAsync(
                            "payment-approved",
                            orderEvent.OrderId.ToString(),
                            new PaymentApprovedEvent(orderEvent.OrderId, orderEvent.ProductId, orderEvent.Quantity)
                        );
                    }
                    else
                    {
                        payment.Status = PaymentStatus.Failed;
                        await _producer.ProduceAsync("payment-failed", orderEvent.OrderId.ToString(), new PaymentFailedEvent(orderEvent.OrderId));
                    }

                    await context.SaveChangesAsync();

                    consumer.Commit(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Payment] Error consuming message: {ex.Message}");
                }
            }
        }
    }
}
