using Swapzy.Core.Entities.Users;

namespace Swapzy.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity> AddAsync(UserEntity user);
    }
}
