using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DbContextSystem _context;

        public InventoryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public IQueryable<InventoryEntity> GetStockByStoreQueryable(int storeId)
        {
            return _context.Inventory
                .AsNoTracking()
                .Include(i => i.Product)
                    .ThenInclude(p => p.Brand)
                .Include(i => i.Product)
                    .ThenInclude(p => p.Category)
                .Where(p => p.IdStore == storeId);
        }

        public async Task<InventoryEntity> GetStockById(int productId, int storeId)
        {
            var stock = await _context.Inventory
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdProduct == productId && x.IdStore == storeId);

            return stock!;
        }

        public async Task<bool> RegisterStockByProducts(InventoryEntity entity)
        {
            await _context.AddAsync(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> UpdateStockByProducts(InventoryEntity entity)
        {
            _context.Update(entity);
            _context.Entry(entity).Property(x => x.Price).IsModified = false;
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }
    }
}
