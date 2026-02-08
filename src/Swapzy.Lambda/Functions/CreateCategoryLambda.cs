using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;
using System.Text.Json;

namespace Swapzy.Lambda.Functions
{
    public class CreateCategoryLambda : LambdaBase
    {
        public async Task<APIGatewayProxyResponse> Handle(
            APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return await ExecuteAsync(request, context,
                async (serviceProvider, userContext) =>
                {
                    var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
                    var dto = JsonSerializer.Deserialize<CreateCategoryDto>(request.Body);

                    if (dto == null)
                        throw new ArgumentException("Invalid request body");

                    var result = await categoryService.CreateAsync(dto, userContext.UserId);
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 201,
                        Body = JsonSerializer.Serialize(new
                        { categoryId = result.Id, category = result }),
                        Headers = new Dictionary<string, string>
                        {
                            ["Content-Type"] = "application/json"
                        }
                    };
                },
                requiresAuth: true);
        }
    }
}
