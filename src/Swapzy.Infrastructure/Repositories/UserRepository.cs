using Microsoft.EntityFrameworkCore;
using Swapzy.Application.Commands.RegisterUser;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities;
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
    }
}
