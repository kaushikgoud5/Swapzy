using Swapzy.Core.Entities.Authorization;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetByIdAsync(Guid id);
       // Task<UserEntity?> GetByIdWithRolesAsync(Guid id);
        Task<UserEntity> AddAsync(UserEntity user);
      //  Task UpdateAsync(UserEntity user);
        Task<bool> EmailExistsAsync(string email);
        Task<UserRole?> AssignRoleAsync(Guid id, Role? userRole);
    }
}
