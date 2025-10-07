namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Declaracion o matricula de nuestras interfaces a nivel repository
        IBrandsRepository Brands { get; }
        ICategoriesRepository Categories { get; }
        IRolesRepository Roles { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
