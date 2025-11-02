using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class RolesRepository : GenericRepository<Roles>, IRolesRepository
    {
        private readonly DbContextSystem _context;

        public RolesRepository(DbContextSystem context) : base(context) 
        { 
            _context = context;
        }

        public async Task<List<Roles>> GetRolesAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Roles> RegisterRoleAsync(Roles role)
        {
            await _context.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;

        }
    }
}
