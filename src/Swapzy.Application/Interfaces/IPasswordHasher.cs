using Swapzy.Core.Entities.Users;

namespace Swapzy.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
        string GenerateJwtToken(UserEntity user);
    }
}
