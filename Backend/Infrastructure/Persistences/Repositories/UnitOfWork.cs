using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextSystem _context;

        public IGenericRepository<BrandEntity> _brand = null!;
        public IGenericRepository<CategoryEntity> _category = null!;
        public IGenericRepository<CustomerEntity> _customer = null!;
        public IGenericRepository<StoreEntity> _store = null!;
        public IGenericRepository<SupplierEntity> _supplier = null!;
        public IModuleRepository _module = null!;
        public IPermissionRepository _permission = null!;
        public IProductRepository _product = null!;
        public IRoleRepository _role = null!;
        public IUserRepository _user = null!;

        public UnitOfWork(DbContextSystem context)
        {
            _context = context;
        }

        public IGenericRepository<BrandEntity> Brand => _brand ?? new GenericRepository<BrandEntity>(_context);
        public IGenericRepository<CategoryEntity> Category => _category ?? new GenericRepository<CategoryEntity>(_context);
        public IGenericRepository<CustomerEntity> Customer => _customer ?? new GenericRepository<CustomerEntity>(_context);
        public IGenericRepository<StoreEntity> Store => _store ?? new GenericRepository<StoreEntity>(_context);
        public IGenericRepository<SupplierEntity> Supplier => _supplier ?? new GenericRepository<SupplierEntity>(_context);
        public IModuleRepository Module => _module ?? new ModuleRepository(_context);
        public IPermissionRepository Permission => _permission ?? new PermissionRepository(_context);
        public IProductRepository Product => _product ?? new ProductRepository(_context);   
        public IRoleRepository Role => _role ?? new RoleRepository(_context);
        public IUserRepository User => _user ?? new UserRepository(_context);

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();
            return transaction.GetDbTransaction();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
