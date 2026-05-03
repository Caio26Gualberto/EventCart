using catalog_service.Context;
using catalog_service.Entities;
using catalog_service.Events;
using catalog_service.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace catalog_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CatalogDbContext _context;
        private readonly KafkaProducer _producer;

        public ProductsController(CatalogDbContext context, KafkaProducer producer)
        {
            _context = context;
            _producer = producer;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Where(product => product.IsActive)
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == id && product.IsActive);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request)
        {
            if (request.InitialQuantity < 0)
            {
                return BadRequest("Initial quantity cannot be negative.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Category = request.Category,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productCreated = new ProductCreatedEvent(product.Id, request.InitialQuantity);

            await _producer.ProduceAsync(
                "product-created",
                product.Id.ToString(),
                productCreated
            );

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
    }
}
