using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAllQueryable();
        Task<IEnumerable<T>> GetSelectAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> RegisterAsync(int userId, T entity);
        Task<bool> EditAsync(int userId, T entity);
        Task<bool> EnableAsync(int userId, int id);
        Task<bool> DisableAsync(int userId, int id);
        Task<bool> RemoveAsync(int userId, int id);
        IQueryable<T> GetEntityQuery(Expression<Func<T, bool>>? filter = null);

    }
}
