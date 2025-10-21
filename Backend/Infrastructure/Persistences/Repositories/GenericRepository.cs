using Domain.Entities;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Utilities.Static;

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
            var getAllQuery = GetEntityQuery(x => x.AUDIT_DELETE_USER == null && x.AUDIT_DELETE_DATE == null);
            return getAllQuery;
        }

        public async Task<IEnumerable<T>> GetSelectAsync()
        {
            var getAll = await _entity
                    .Where(x => Convert.ToInt32(x.STATE).Equals((int)StateTypes.ACTIVE) && x.AUDIT_DELETE_USER == null && x.AUDIT_DELETE_DATE == null).AsNoTracking().ToListAsync();

            return getAll;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var getById = await _entity.FindAsync(id);

            if (getById != null)
            {
                _context.Entry(getById).State = EntityState.Detached;
            }

            return getById!;
        }

        public async Task<bool> RegisterAsync(T entity)
        {
            entity.AUDIT_CREATE_USER = 1;
            entity.AUDIT_CREATE_DATE = DateTime.Now;
            entity.STATE = true;

            await _context.AddAsync(entity);

            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EditAsync(T entity)
        {
            entity.AUDIT_UPDATE_USER = 1;
            entity.AUDIT_UPDATE_DATE = DateTime.Now;

            _context.Update(entity);
            _context.Entry(entity).Property(x => x.STATE).IsModified = false;
            _context.Entry(entity).Property(x => x.AUDIT_CREATE_USER).IsModified = false;
            _context.Entry(entity).Property(x => x.AUDIT_CREATE_DATE).IsModified = false;
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> EnableAsync(int id)
        {

            T entity = await GetByIdAsync(id);

            entity!.AUDIT_UPDATE_USER = 1;
            entity.AUDIT_UPDATE_DATE = DateTime.Now;
            entity.STATE = true;

            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> DisableAsync(int id)
        {
            T entity = await GetByIdAsync(id);

            entity!.AUDIT_UPDATE_USER = 1;
            entity.AUDIT_UPDATE_DATE = DateTime.Now;
            entity.STATE = false;

            _context.Update(entity);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            T entity = await GetByIdAsync(id);

            entity!.AUDIT_DELETE_USER = 1;
            entity.AUDIT_DELETE_DATE = DateTime.Now;
            entity.STATE = false;
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
