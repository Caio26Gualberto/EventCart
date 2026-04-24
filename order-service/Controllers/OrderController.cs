using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.Entities;
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

            Order order = new Order
            {
                Id = orderId,
                ProductId = Guid.NewGuid(),
                Quantity = Random.Shared.Next(1, 10)
            };

            var orderEvent = new OrderCreatedEvent(
                orderId,
                order.ProductId,
                order.Quantity
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
