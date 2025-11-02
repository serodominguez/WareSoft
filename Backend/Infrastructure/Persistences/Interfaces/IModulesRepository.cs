using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IModulesRepository : IGenericRepository<Modules>
    {
        Task<List<Modules>> GetModulesAsync();
        public Task<Modules> RegisterModuleAsync(Modules module);
    }
}
