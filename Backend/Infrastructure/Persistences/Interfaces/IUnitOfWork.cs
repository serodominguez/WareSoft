using Domain.Entities;
using System.Data;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<BrandEntity> Brand { get; }
        IGenericRepository<CategoryEntity> Category { get; }
        IGenericRepository<StoreEntity> Store { get; }
        IModuleRepository Module { get; }
        IPermissionRepository Permission { get; }
        IProductRepository Product { get; }
        IRoleRepository Role { get; }
        IUserRepository User { get; }

        void SaveChanges();
        Task SaveChangesAsync();

        IDbTransaction BeginTransaction();
    }
}
