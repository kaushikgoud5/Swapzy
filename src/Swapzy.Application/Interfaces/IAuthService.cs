using Swapzy.Application.DTOs;
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
        Task<Guid> RegisterAsync(RegisterUserDto dto);
        Task<string> LoginAsync(LoginUserDto dto);
    }
    
}
