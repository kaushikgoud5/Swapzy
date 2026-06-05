using System.Net.Http.Headers;
using System.Text.Json;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Configurations;

namespace Swapzy.Infrastructure.Services
{
    public class SocialAuthService : ISocialAuthService
    {
        private readonly SocialAuthSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public SocialAuthService(IOptions<SocialAuthSettings> settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<SocialUserInfo> ValidateGoogleTokenAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _settings.GoogleClientId }
            });

            return new SocialUserInfo
            {
                Email = payload.Email,
                Name = payload.Name ?? payload.Email.Split('@')[0]
            };
        }

        public async Task<SocialUserInfo> ValidateGitHubCodeAsync(string code)
        {
            var client = _httpClientFactory.CreateClient();

            // Exchange code for access token
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _settings.GitHubClientId,
                ["client_secret"] = _settings.GitHubClientSecret,
                ["code"] = code
            });
            tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tokenResponse = await client.SendAsync(tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var accessToken = JsonDocument.Parse(tokenJson).RootElement.GetProperty("access_token").GetString()
                ?? throw new Exception("Failed to get GitHub access token");

            // Get user info
            var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            userRequest.Headers.UserAgent.ParseAdd("Swapzy");

            var userResponse = await client.SendAsync(userRequest);
            userResponse.EnsureSuccessStatusCode();
            var userJson = await userResponse.Content.ReadAsStringAsync();
            var userDoc = JsonDocument.Parse(userJson).RootElement;

            var name = userDoc.TryGetProperty("name", out var n) && n.ValueKind != JsonValueKind.Null
                ? n.GetString() : userDoc.GetProperty("login").GetString();

            // Get email (may be private)
            var email = userDoc.TryGetProperty("email", out var e) && e.ValueKind != JsonValueKind.Null
                ? e.GetString() : null;

            if (string.IsNullOrEmpty(email))
            {
                var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                emailRequest.Headers.UserAgent.ParseAdd("Swapzy");

                var emailResponse = await client.SendAsync(emailRequest);
                emailResponse.EnsureSuccessStatusCode();
                var emailsJson = await emailResponse.Content.ReadAsStringAsync();
                var emails = JsonDocument.Parse(emailsJson).RootElement;

                foreach (var item in emails.EnumerateArray())
                {
                    if (item.TryGetProperty("primary", out var primary) && primary.GetBoolean())
                    {
                        email = item.GetProperty("email").GetString();
                        break;
                    }
                }

                email ??= emails.EnumerateArray().First().GetProperty("email").GetString();
            }

            return new SocialUserInfo
            {
                Email = email!,
                Name = name ?? email!.Split('@')[0]
            };
        }
    }
}
