using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsReceiptDetailsRepository : IGoodsReceiptDetailsRepository
    {
        private readonly DbContextSystem _context;

        public GoodsReceiptDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GoodsReceiptDetailsEntity>> GetGoodsReceiptDetailsAsync(int receiptId)
        {
            var response = await _context.Product
                .AsNoTracking()
                .Join(_context.GoodsReceiptDetails, p => p.Id, rd => rd.IdProduct, (p, rd)
                        => new { ProductEntity = p, GoodsReceiptDetailsEntity = rd })
                .Join(_context.Brand, x => x.ProductEntity.IdBrand, b => b.Id, (x, b)
                        => new { x.ProductEntity, x.GoodsReceiptDetailsEntity, BrandEntity = b })
                .Join(_context.Category, x => x.ProductEntity.IdCategory, c => c.Id, (x, c)
                        => new { x.ProductEntity, x.GoodsReceiptDetailsEntity, x.BrandEntity, CategoryEntity = c})
                .Where(x => x.GoodsReceiptDetailsEntity.IdReceipt == receiptId)
                .Select(x => new GoodsReceiptDetailsEntity
                {
                    IdReceipt = x.GoodsReceiptDetailsEntity.IdReceipt,
                    Item = x.GoodsReceiptDetailsEntity.Item,
                    IdProduct = x.ProductEntity.Id,
                    Product = new ProductEntity
                    {
                        Code = x.ProductEntity.Code,
                        Description = x.ProductEntity.Description,
                        Material = x.ProductEntity.Material,
                        Color = x.ProductEntity.Color,
                        Brand = new BrandEntity
                        {
                            BrandName = x.BrandEntity.BrandName
                        },
                        Category = new CategoryEntity
                        {
                            CategoryName = x.CategoryEntity.CategoryName
                        }
                    },
                    Quantity = x.GoodsReceiptDetailsEntity.Quantity,
                    UnitCost = x.GoodsReceiptDetailsEntity.UnitCost,
                    TotalCost = x.GoodsReceiptDetailsEntity.TotalCost
                })
                .ToListAsync();

            return response;
        }
    }
}
