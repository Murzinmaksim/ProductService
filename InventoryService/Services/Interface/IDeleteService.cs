namespace InventoryService.Services.Interface
{
    public interface IDeleteService
    {
        Task DeleteInventoryAsync(int inventoryId);
    }
}
