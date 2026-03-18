using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;

namespace Swapzy.Lambda.Functions.Handlers
{
    public class RegisterUserHandler : LambdaBase, IRouteHandler
    {
        public bool RequiresAuth => false;

        public async Task<APIGatewayProxyResponse> HandleAsync(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            return await ExecuteAsync(request, context, async (sp, _) =>
            {
                var authService = sp.GetRequiredService<IAuthService>();
                var dto = Deserialize<RegisterRequestDto>(request.Body);
                var userId = await authService.RegisterAsync(dto);
                return Ok(new { UserId = userId }, 201);
            }, RequiresAuth);
        }
    }
}
