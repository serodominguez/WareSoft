using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GoodsIssueDetailsRepository : IGoodsIssueDetailsRepository
    {
        private readonly DbContextSystem _context;

        public GoodsIssueDetailsRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GoodsIssueDetailsEntity>> GetGoodsIssueDetailsAsync(int issueId)
        {
            var response = await _context.Product
                .AsNoTracking()
                .Join(_context.GoodsIssueDetails, p => p.Id, rd => rd.IdProduct, (p, rd)
                        => new { ProductEntity = p, GoodsIssueDetailsEntity = rd })
                .Join(_context.Brand, x => x.ProductEntity.IdBrand, b => b.Id, (x, b)
                        => new { x.ProductEntity, x.GoodsIssueDetailsEntity, BrandEntity = b })
                .Join(_context.Category, x => x.ProductEntity.IdCategory, c => c.Id, (x, c)
                        => new { x.ProductEntity, x.GoodsIssueDetailsEntity, x.BrandEntity, CategoryEntity = c })
                .Where(x => x.GoodsIssueDetailsEntity.IdIssue == issueId)
                .Select(x => new GoodsIssueDetailsEntity
                {
                    IdIssue = x.GoodsIssueDetailsEntity.IdIssue,
                    Item = x.GoodsIssueDetailsEntity.Item,
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
                    Quantity = x.GoodsIssueDetailsEntity.Quantity,
                    UnitPrice = x.GoodsIssueDetailsEntity.UnitPrice,
                    TotalPrice = x.GoodsIssueDetailsEntity.TotalPrice
                })
                .ToListAsync();

            return response;
        }
    }
}
