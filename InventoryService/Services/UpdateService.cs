using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly InventoryServiceContext _context;

        public UpdateService(InventoryServiceContext context)
        {
            _context = context;
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            if (inventory == null || inventory.id == 0)
            {
                throw new ArgumentException("Invalid inventory data.");
            }

            var existingInventory = await _context.Inventory.FindAsync(inventory.id);
            if (existingInventory == null)
            {
                throw new KeyNotFoundException($"Inventory item with id {inventory.id} not found.");
            }

            existingInventory.quantity = inventory.quantity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryExists(inventory.id))
                {
                    throw new KeyNotFoundException($"Inventory item with id {inventory.id} not found.");
                }
                else
                {
                    throw;
                }
            }
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventory.Any(e => e.id == id);
        }
    }
}
