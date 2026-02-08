using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Swapzy.Infrastructure.Services
{
    public class JwtAuthMiddleware : IAuthMiddleware
    {
        private readonly IConfiguration _configuration;
        public JwtAuthMiddleware(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<UserContext> AuthenticateAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (!request.Headers.TryGetValue("Authorization", out var authHeader))
                throw new UnauthorizedAccessException("Missing Authorization header");

            if (!authHeader.StartsWith("Bearer "))
                throw new UnauthorizedAccessException("Invalid Authorization scheme");

            var token = authHeader["Bearer ".Length..];

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var principal = handler.ValidateToken(
                token,
                validationParameters,
                out _);
            var userId = Guid.Parse(
            principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

            var roles = principal.FindAll("role").Select(c => c.Value).ToList();
            var permissions = principal.FindAll("perm").Select(c => c.Value).ToList();

            return Task.FromResult(new UserContext
            {
                UserId = userId,
                Roles = roles,
                Permissions = permissions
            });
        }
    }
}
