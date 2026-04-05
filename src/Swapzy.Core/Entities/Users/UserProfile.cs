using Swapzy.Core.Common;

namespace Swapzy.Core.Entities.Users
{
    public class UserProfile : BaseAuditableEntity
    {
        public Guid UserId { get; set; }
        public string? AvatarUrl { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public UserEntity User { get; set; } = null!;
    }
}
