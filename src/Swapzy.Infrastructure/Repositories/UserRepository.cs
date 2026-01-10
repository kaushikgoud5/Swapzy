using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Authorization;
using Swapzy.Core.Entities.Users;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SwapzyDbContext _swapzyDbContext;
        public UserRepository(SwapzyDbContext swapzyDbContext)
        {
            _swapzyDbContext = swapzyDbContext;
        }
        public async Task<UserEntity> AddAsync(UserEntity user)
        {
            _swapzyDbContext.Users.Add(user);
            await _swapzyDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _swapzyDbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<UserEntity?> GetByIdAsync(Guid id)
        {
            return await _swapzyDbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(UserEntity user)
        {
            user.ModifiedOn = DateTime.UtcNow;
            _swapzyDbContext.Users.Update(user);
            await _swapzyDbContext.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _swapzyDbContext.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<UserRole?> AssignRoleAsync(Guid id, Role role)
        {
            var userRole = new UserRole
            {
                UserId = id,
                RoleId = role.Id,
                CreatedOn = DateTime.UtcNow,
            };
            await _swapzyDbContext.UserRoles.AddAsync(userRole);
            await _swapzyDbContext.SaveChangesAsync();
            return userRole;
        }
    }
}
