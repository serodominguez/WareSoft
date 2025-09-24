namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Declaracion o matricula de nuestras interfaces a nivel repository
        ICategoriesRepository Categories { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
