using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.DTOs.Responses
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public AuthUserDto User { get; set; }
    }

    public class AuthUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
