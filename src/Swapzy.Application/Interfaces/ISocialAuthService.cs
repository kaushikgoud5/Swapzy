namespace Swapzy.Application.Interfaces
{
    public class SocialUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public interface ISocialAuthService
    {
        Task<SocialUserInfo> ValidateGoogleTokenAsync(string idToken);
        Task<SocialUserInfo> ValidateGitHubCodeAsync(string code);
    }
}
