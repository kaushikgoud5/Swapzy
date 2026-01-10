using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Configurations;
using Swapzy.Core.Entities.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Swapzy.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRoleRepository _roleRepository;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public JwtTokenService(
            IOptions<JwtSettings> jwtSettings,
            IRoleRepository roleRepository,
            IDistributedCache cache,
            IConfiguration configuration)
        {
            _jwtSettings = jwtSettings.Value;
            _roleRepository = roleRepository;
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<string> GenerateAccessTokenAsync(UserEntity user)
        {
            var roles = await _roleRepository.GetUserRolesAsync(user.Id);
            var permissions = await _roleRepository.GetUserPermissionsAsync(user.Id);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permissions", permission));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
               issuer: _configuration["Jwt:Issuer"],
               audience: _configuration["Jwt:Audience"],
               claims: claims,
               expires: DateTime.UtcNow.AddHours(2),
               signingCredentials: credentials
           );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var cacheKey = $"refresh_token:{userId}:{refreshToken}";
            var cachedToken = await _cache.GetStringAsync(cacheKey);
            return cachedToken != null;
        }

        public async Task StoreRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresAt)
        {
            var cacheKey = $"refresh_token:{userId}:{refreshToken}";
            var tokenData = new
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expiresAt
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(tokenData),
                options);

            // Also store in a user-specific list for revoking all tokens
            var userTokensKey = $"user_tokens:{userId}";
            var userTokens = await GetUserRefreshTokensAsync(userId);
            userTokens.Add(refreshToken);

            await _cache.SetStringAsync(
                userTokensKey,
                JsonSerializer.Serialize(userTokens),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
                });
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var cacheKey = $"refresh_token:{userId}:{refreshToken}";
            await _cache.RemoveAsync(cacheKey);

            // Remove from user tokens list
            var userTokensKey = $"user_tokens:{userId}";
            var userTokens = await GetUserRefreshTokensAsync(userId);
            userTokens.Remove(refreshToken);

            await _cache.SetStringAsync(
                userTokensKey,
                JsonSerializer.Serialize(userTokens));
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            var userTokens = await GetUserRefreshTokensAsync(userId);

            foreach (var token in userTokens)
            {
                var cacheKey = $"refresh_token:{userId}:{token}";
                await _cache.RemoveAsync(cacheKey);
            }

            var userTokensKey = $"user_tokens:{userId}";
            await _cache.RemoveAsync(userTokensKey);
        }

        private async Task<List<string>> GetUserRefreshTokensAsync(Guid userId)
        {
            var userTokensKey = $"user_tokens:{userId}";
            var cachedTokens = await _cache.GetStringAsync(userTokensKey);

            if (string.IsNullOrEmpty(cachedTokens))
                return new List<string>();

            return JsonSerializer.Deserialize<List<string>>(cachedTokens) ?? new List<string>();
        }
    }
}
