using Swapzy.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessTokenAsync(UserEntity user);
        Task<string> GenerateRefreshTokenAsync();
        ClaimsPrincipal? ValidateToken(string token);
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
        Task StoreRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiresAt);
        Task RevokeRefreshTokenAsync(Guid userId, string refreshToken);
        Task RevokeAllUserTokensAsync(Guid userId);
    }
}
