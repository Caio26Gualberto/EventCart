using Confluent.Kafka;
using System.Runtime.InteropServices;

namespace order_service
{
    public class KafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync(string topic, string key, object value)
        {
            var message = new Message<string, string>
            {
                Key = key,
                Value = System.Text.Json.JsonSerializer.Serialize(value)
            };

            await _producer.ProduceAsync(topic, message);
        }
    }
}
