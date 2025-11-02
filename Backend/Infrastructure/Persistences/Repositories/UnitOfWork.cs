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


        public IGenericRepository<Brands> _brands = null!;
        public IGenericRepository<Categories> _categories = null!; 
        public IGenericRepository<Stores> _stores = null!;
        public IModulesRepository _modules = null!;
        public IPermissionsRepository _permissions = null!;
        public IRolesRepository _roles = null!;
        public IUsersRepository _users = null!;

        public UnitOfWork(DbContextSystem context)
        {
            _context = context;
        }

        public IGenericRepository<Brands> Brands => _brands ?? new GenericRepository<Brands>(_context);
        public IGenericRepository<Categories> Categories => _categories ?? new GenericRepository<Categories>(_context);
        public IGenericRepository<Stores> Stores => _stores ?? new GenericRepository<Stores>(_context);
        public IModulesRepository Modules => _modules ?? new ModulesRepository(_context);
        public IPermissionsRepository Permissions => _permissions ?? new PermissionsRepository(_context);
        public IRolesRepository  Roles => _roles ?? new RolesRepository(_context);
        public IUsersRepository Users => _users ?? new UsersRepository(_context);


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
