using Swapzy.Core.Constants.Authorization;

namespace Swapzy.Application.DTOs.Requests
{
    public class RegisterRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }

        public string Role = Roles.User;
    }
}
