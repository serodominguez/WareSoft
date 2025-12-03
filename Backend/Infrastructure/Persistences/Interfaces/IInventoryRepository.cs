using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryEntity> GetStockById(int productId, int storeId);
        Task<bool> RegisterStockByProducts(InventoryEntity entity);
        Task<bool> UpdateStockByProducts(InventoryEntity entity);
    }
}
