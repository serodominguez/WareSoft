using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ActionsRepository : GenericRepository<Actions>, IActionsRepository
    {
        private readonly DbContextSystem _context;

        public ActionsRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Actions>> GetActionsAsync()
        {
            return await _context.Actions
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
