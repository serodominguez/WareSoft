using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IRolesRepository : IGenericRepository<Roles>
    {
        Task<List<Roles>> GetRolesAsync();
        public Task<Roles> RegisterRoleAsync(Roles role);
    }
}
