using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using System.Text.Json;

namespace Swapzy.Lambda.Functions
{
    public class LoginUserLambda : LambdaBase
    {
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest request,
        ILambdaContext context)
        {
            return await ExecuteAsync(async sp =>
            {
                var authService = sp.GetRequiredService<IAuthService>();
                var loginDto = JsonSerializer.Deserialize<LoginRequestDto>(request.Body);
                var respone = await authService.LoginAsync(loginDto);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(respone),
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    }
                };
            }, context);
        }
    }
}
