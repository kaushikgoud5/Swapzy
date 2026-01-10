using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.DTOs.Responses
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }

        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
