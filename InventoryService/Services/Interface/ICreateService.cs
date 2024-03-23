using InventoryService.Models;

namespace InventoryService.Services.Interface
{
    public interface ICreateService
    {
        Task CreateInventoryAsync(Inventory inventory);
    }
}
