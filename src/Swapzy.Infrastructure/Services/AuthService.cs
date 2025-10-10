using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities;

namespace Swapzy.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<UserEntity> _passwordHasher;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<UserEntity>();
        }
        public string GenerateJwtToken(UserEntity user)
        {
            throw new NotImplementedException();
        }

        public string HashPassword(string password)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            throw new NotImplementedException();
        }
    }
}
