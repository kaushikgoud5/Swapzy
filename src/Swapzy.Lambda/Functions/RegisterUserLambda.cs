using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Swapzy.Lambda.Functions
{

    public class RegisterUserLambda : LambdaBase
    {
        public async Task<APIGatewayProxyResponse> Handle(
            APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return await ExecuteAsync(request, context, 
                async (sp, _) => {

                var authService = sp.GetRequiredService<IAuthService>();

                var dto = JsonSerializer.Deserialize<RegisterRequestDto>(
                    request.Body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var userId = await authService.RegisterAsync(dto);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 201,
                    Body = JsonSerializer.Serialize(new
                    {
                        UserId = userId
                    }),
                    Headers = new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json"
                    }
                };
            },
                false
            );

        }

    }

}
