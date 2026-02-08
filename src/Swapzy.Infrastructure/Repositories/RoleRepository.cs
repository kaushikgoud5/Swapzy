using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Authorization;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly SwapzyDbContext _context;

        public RoleRepository(SwapzyDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Role>> GetUserRolesAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();
        }

    }
}
