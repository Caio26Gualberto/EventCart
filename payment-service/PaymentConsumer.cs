using Confluent.Kafka;
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
        private readonly PaymentDbContext _context;
        public PaymentConsumer(KafkaProducer producer, PaymentDbContext context)
        {
            _producer = producer;
            _context = context;
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

                    Payment payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderEvent!.OrderId,
                        Status = PaymentStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"[Payment] Processing Order: {orderEvent!.OrderId}");

                    var success = Random.Shared.Next(0, 2) == 1; // Randomly decide if payment is successful

                    if (success)
                    {
                        payment.Status = PaymentStatus.Approved;
                        _context.Payments.Update(payment);
                        await _producer.ProduceAsync("payment-approved", orderEvent.OrderId.ToString(), new PaymentApprovedEvent(orderEvent.OrderId));
                    }
                    else
                    {
                        payment.Status = PaymentStatus.Failed;
                        _context.Payments.Update(payment);
                        await _producer.ProduceAsync("payment-failed", orderEvent.OrderId.ToString(), new PaymentFailedEvent(orderEvent.OrderId));
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Payment] Error consuming message: {ex.Message}");
                }
            }
        }
    }
}
