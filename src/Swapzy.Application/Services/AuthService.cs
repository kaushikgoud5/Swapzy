using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Swapzy.Application.DTO_S;
using Swapzy.Application.DTOs;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace Swapzy.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Guid> RegisterAsync(RegisterUserDto dto)
        {

            var user = new UserEntity { Name = dto.Username, HashedPassword = dto.Password };
            await _userRepository.AddAsync(user);

            return user.Id;
        }

        public async Task<string> LoginAsync(LoginUserDto dto)
        {
           throw new NotImplementedException();
        }
    }
}
