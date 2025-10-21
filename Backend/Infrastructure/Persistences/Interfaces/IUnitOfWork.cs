using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Declaracion o matricula de nuestras interfaces a nivel repository
        IGenericRepository<Brands> Brands { get; }
        IGenericRepository<Categories> Categories { get; }
        IGenericRepository<Roles> Roles { get; }
        IGenericRepository<Stores> Stores { get; }
        IUsersRepository Users { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
