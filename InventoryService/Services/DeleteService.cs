using InventoryService.Data;
using InventoryService.Services.Interface;

namespace InventoryService.Services
{
    public class DeleteService : IDeleteService
    {
        private readonly InventoryServiceContext _context;

        public DeleteService(InventoryServiceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task DeleteInventoryAsync(int inventoryId)
        {
            // Проверка входного параметра на корректность
            if (inventoryId <= 0)
            {
                throw new ArgumentException("Inventory ID must be greater than zero.", nameof(inventoryId));
            }

            // Поиск записи инвентаря по идентификатору
            var inventory = await _context.Inventory.FindAsync(inventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {inventoryId} not found.");
            }

            // Удаление записи из контекста
            _context.Inventory.Remove(inventory);

            // Сохранение изменений в базе данных
            await _context.SaveChangesAsync();
        }
    }
}
