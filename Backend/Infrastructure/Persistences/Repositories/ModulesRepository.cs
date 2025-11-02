using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ModulesRepository : GenericRepository<Modules>, IModulesRepository
    {
        private readonly DbContextSystem _context;

        public ModulesRepository(DbContextSystem context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Modules>> GetModulesAsync()
        {
            return await _context.Modules
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Modules> RegisterModuleAsync(Modules module)
        {
            await _context.AddAsync(module);
            await _context.SaveChangesAsync();
            return module;

        }
    }
}
