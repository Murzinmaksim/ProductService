using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryServiceContext _context;
        private readonly ICreateService _createService;
        private readonly IUpdateService _updateService;
        private readonly IDeleteService _deleteService;

        public InventoryController(InventoryServiceContext context, ICreateService createService, IUpdateService updateService, IDeleteService deleteService)
        {
            _context = context;
            _createService = createService;
            _updateService = updateService;
            _deleteService = deleteService;
        }

        // GET: api/Inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventory()
        {
            return await _context.Inventory.ToListAsync();
        }

        // GET: api/Inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventory(int id)
        {
            var inventoryItem = await _context.Inventory.FindAsync(id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            return inventoryItem;
        }

        // POST: api/Inventory
        [HttpPost]
        public async Task<ActionResult<Inventory>> PostInventory(Inventory inventory)
        {
            try
            {
                await _createService.CreateInventoryAsync(inventory);
                return CreatedAtAction(nameof(GetInventory), new { id = inventory.id }, inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the inventory. {ex.Message}");
            }
        }

        // PUT: api/Inventory
        [HttpPut]
        public async Task<IActionResult> PutInventory(Inventory inventory)
        {
            try
            {
                await _updateService.UpdateInventoryAsync(inventory);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the inventory. {ex.Message}");
            }
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            try
            {
                await _deleteService.DeleteInventoryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the inventory. {ex.Message}");
            }
        }
    }
}
