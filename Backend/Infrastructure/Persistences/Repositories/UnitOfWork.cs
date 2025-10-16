using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextSystem _context;
        public IBrandsRepository Brands { get; private set; }
        public ICategoriesRepository Categories { get; private set; }
        public IRolesRepository Roles { get; private set; }
        public IStoresRepository Stores { get; private set; }
        public IUsersRepository Users { get; private set; }

        public UnitOfWork(DbContextSystem context)
        {
            _context = context;
            Brands = new BrandsRepository(_context);
            Categories = new CategoriesRepository(_context);
            Roles = new RolesRepository(_context);
            Stores = new StoresRepository(_context);
            Users = new UsersRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
