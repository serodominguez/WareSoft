using Domain.Entities;
using System.Data;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Brands> Brands { get; }
        IGenericRepository<Categories> Categories { get; }
        IGenericRepository<Stores> Stores { get; }
        IModulesRepository Modules { get; }
        IPermissionsRepository Permissions { get; }
        IRolesRepository Roles { get; }
        IUsersRepository Users { get; }
        
        void SaveChanges();
        Task SaveChangesAsync();

        IDbTransaction BeginTransaction();
    }
}
