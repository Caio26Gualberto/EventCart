using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_service.Context;
using order_service.Entities;
using order_service.Events;
using order_service.Requests;

namespace order_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly KafkaProducer _producer;
        private readonly OrderDbContext _context;
        public OrderController(KafkaProducer producer, OrderDbContext context)
        {
            _producer = producer;
            _context = context;
        }

        //TODO criar serviço de criação de pedido para persistir no banco de dados e enviar o evento para o Kafka

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest? request)
        {
            var productId = request?.ProductId == Guid.Empty || request?.ProductId == null
                ? Guid.NewGuid()
                : request.ProductId;

            var quantity = request?.Quantity > 0
                ? request.Quantity
                : Random.Shared.Next(1, 10);

            var orderId = Guid.NewGuid();

            Order order = new Order
            {
                Id = orderId,
                ProductId = productId,
                Quantity = quantity
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

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
