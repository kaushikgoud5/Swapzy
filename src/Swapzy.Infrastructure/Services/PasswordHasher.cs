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
        private readonly IConfiguration _configuration;

        public PasswordHasher(IPasswordHasher<UserEntity> passwordHasher, IConfiguration configuration)
        {
            _passwordHasher = passwordHasher;
            _configuration = configuration;
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


        public string GenerateJwtToken(UserEntity user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
