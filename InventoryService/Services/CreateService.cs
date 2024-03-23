using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Services.Interface;

namespace InventoryService.Services
{
    public class CreateService : ICreateService
    {
        private readonly InventoryServiceContext _context;

        public CreateService(InventoryServiceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateInventoryAsync(Inventory inventory)
        {
            // Проверка на null и другие валидации могут быть добавлены здесь
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            // Добавление нового инвентаря в контекст и сохранение изменений в базу данных
            _context.Inventory.Add(inventory);
            await _context.SaveChangesAsync();
        }
    }
}
