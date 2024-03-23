using InventoryService.Models;

namespace InventoryService.Services.Interface
{
    public interface IUpdateService
    {
        Task UpdateInventoryAsync(Inventory inventory);
    }
}
