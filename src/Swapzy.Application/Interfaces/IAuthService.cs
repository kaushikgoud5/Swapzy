using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(Guid userId, string refreshToken);
        Task<bool> RevokeAllTokensAsync(Guid userId);
    }
    
}
