using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAllQueryable();
        Task<IEnumerable<T>> GetSelectAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> RegisterAsync(T entity);
        Task<bool> EditAsync(T entity);
        Task<bool> EnableAsync(int id);
        Task<bool> DisableAsync(int id);
        Task<bool> RemoveAsync(int id);
        IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null);

    }
}
