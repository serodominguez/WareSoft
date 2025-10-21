﻿using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextSystem _context;

        public IGenericRepository<Brands> _brands = null!;

        public IGenericRepository<Categories> _categories = null!;

        public IGenericRepository<Roles> _roles = null!;

        public IGenericRepository<Stores> _stores = null!;

        public IUsersRepository _users = null!;

        public UnitOfWork(DbContextSystem context)
        {
            _context = context;
        }

        public IGenericRepository<Brands> Brands => _brands ?? new GenericRepository<Brands>(_context);
        public IGenericRepository<Categories> Categories => _categories ?? new GenericRepository<Categories>(_context);
        public IGenericRepository<Roles> Roles => _roles ?? new GenericRepository<Roles>(_context);
        public IGenericRepository<Stores> Stores => _stores ?? new GenericRepository<Stores>(_context);
        public IUsersRepository Users => _users ?? new UsersRepository(_context);

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
