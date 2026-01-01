using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.DTOs;
using Swapzy.Application.Interfaces;
using System.Text.Json;

namespace Swapzy.Lambda.Functions
{
    public class RegisterUserLambda : LambdaBase
    {
        public async Task<APIGatewayProxyResponse> Handle(
        APIGatewayProxyRequest request,
        ILambdaContext context)
        {
            return await ExecuteAsync(async sp =>
            {
                var authService = sp.GetRequiredService<IAuthService>();
                var dto = JsonSerializer.Deserialize<RegisterUserDto>(request.Body);
                var userId = await authService.RegisterAsync(dto);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(userId),
                    Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
                };
            }, context);
        }
    }
}
