using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.Events;

namespace order_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly KafkaProducer _producer;
        public OrderController(KafkaProducer producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();

            var orderEvent = new OrderCreatedEvent(
                orderId,
                Guid.NewGuid(),
                1
            );

            await _producer.ProduceAsync(
                "order-created",
                orderId.ToString(),
                orderEvent
            );

            Console.WriteLine($"Order created: {orderId}");

            return Ok(new { orderId });
        }
    }
}
