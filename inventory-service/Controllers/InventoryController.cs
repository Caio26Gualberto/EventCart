using inventory_service.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inventory_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public InventoryController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            var inventory = await _context.InventoryItems.ToListAsync();

            return Ok(inventory);
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetInventoryByProduct(Guid productId)
        {
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(item => item.ProductId == productId);

            if (inventoryItem is null)
            {
                return NotFound();
            }

            return Ok(inventoryItem);
        }
    }
}
