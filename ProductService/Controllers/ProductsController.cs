using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Services.Interface;
using System.Text.Json;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductServiceContext _context;
        private readonly IRabbitMQService _rabbitMQService;


        public ProductsController(ProductServiceContext context, IRabbitMQService rabbitMQService)
        {
            _context = context;
            _rabbitMQService = rabbitMQService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Возвращаем 201 Created и информацию о созданном товаре
            return CreatedAtAction(nameof(GetProduct), new { id = product.id }, product);
        }

        // PUT: api/Products
        [HttpPut]
        public async Task<IActionResult> PutProduct(Product product)
        {
            if (product == null || product.id == 0)
            {
                return BadRequest("Invalid product data.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Возвращаем 204 No Content
            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/inventory
        [HttpPost("inventory")]
        public async Task<IActionResult> Post([FromBody] Inventory inventory)
        {
            var message = JsonSerializer.Serialize(inventory);
            var routingKey = "inventory.product.post"; // Изменение ключа маршрутизации для POST-запроса
            await Task.Run(() => _rabbitMQService.SendMessage(message, routingKey));
            return StatusCode(201); // Возвращаем код 201 (Created) без дополнительных данных
        }

        // PUT: api/inventory
        [HttpPut("inventory")]
        public async Task<IActionResult> Put([FromBody] Inventory inventory)
        {
            var message = JsonSerializer.Serialize(inventory);
            var routingKey = "inventory.product.put"; 
            await Task.Run(() => _rabbitMQService.SendMessage(message, routingKey)); 
            return NoContent();
        }

        // DELETE: api/inventory/{id}
        [HttpDelete("inventory/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = JsonSerializer.Serialize(new { Id = id });
            var routingKey = "inventory.product.delete";
            await Task.Run(() => _rabbitMQService.SendMessage(message, routingKey));
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.id == id);
        }
    }
}
