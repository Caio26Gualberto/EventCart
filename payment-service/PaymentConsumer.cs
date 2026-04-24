using Confluent.Kafka;
using payment_service.Events;
using System.Text.Json;

namespace payment_service
{
    public class PaymentConsumer : BackgroundService
    {
        private readonly KafkaProducer _producer;
        public PaymentConsumer(KafkaProducer producer)
        {
            _producer = producer;
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

                    Console.WriteLine($"[Payment] Processing Order: {orderEvent!.OrderId}");

                    var success = Random.Shared.Next(0, 2) == 1; // Randomly decide if payment is successful

                    if (success)
                    {
                        await _producer.ProduceAsync("payment-approved", orderEvent.OrderId.ToString(), new PaymentApprovedEvent(orderEvent.OrderId));
                    }
                    else
                    {
                        await _producer.ProduceAsync("payment-failed", orderEvent.OrderId.ToString(), new PaymentFailedEvent(orderEvent.OrderId));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Payment] Error consuming message: {ex.Message}");
                }
            }
        }
    }
}
