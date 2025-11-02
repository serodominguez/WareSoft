using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IActionsRepository : IGenericRepository<Actions>
    {
        Task<List<Actions>> GetActionsAsync();
    }
}
