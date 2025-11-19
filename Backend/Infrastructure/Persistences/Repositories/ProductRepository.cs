using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ProductRepository : GenericRepository<ProductEntity>, IProductRepository
    {
        private readonly DbContextSystem _context;

        public ProductRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public IQueryable<ProductEntity> GetProductsQueryable()
        {
            return _context.Product
                .Include(u => u.Brand)
                .Include(u => u.Category)
                .AsNoTracking();
        }
    }
}
