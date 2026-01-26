using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IInventoryRepository
    {
        IQueryable<InventoryEntity> GetInventoryByStoreQueryable(int storeId);
        Task<InventoryEntity> GetStockByIdAsync(int productId, int storeId);
        Task<bool> RegisterStockByProductsAsync(InventoryEntity entity);
        Task<bool> UpdateStockByProductsAsync(InventoryEntity entity);
        Task<bool> UpdatePriceByProductsAsync(InventoryEntity entity);
    }
}
