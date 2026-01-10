using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swapzy.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        public PasswordHasher(IPasswordHasher<UserEntity> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }
        public string Hash(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                passwordHash,
                password);

            return result == PasswordVerificationResult.Success;
        }
    }
}
