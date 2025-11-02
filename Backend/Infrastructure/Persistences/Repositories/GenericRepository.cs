using Domain.Entities;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _entity;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        public IQueryable<T> GetAllQueryable()
        {
            var getAllQuery = GetEntityQuery();
            return getAllQuery;
        }

        public async Task<IEnumerable<T>> GetSelectAsync()
        {
            var getAll = await _entity
                    .AsNoTracking()
                    .ToListAsync();
            return getAll;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _entity.AsNoTracking().FirstOrDefaultAsync(x => x.PK_ENTITY.Equals(id));

            return entity;
        }

        public async Task<bool> RegisterAsync(T entity)
        {
            await _context.AddAsync(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditAsync(T entity)
        {
            _context.Update(entity);
            _context.Entry(entity).Property(x => x.STATE).IsModified = false;
            _context.Entry(entity).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
            _context.Entry(entity).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _entity;
            if (filter != null) query = query.Where(filter);
            return query;
        }
    }
}
