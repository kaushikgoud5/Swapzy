using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;
using System.Text.Json;

namespace Swapzy.Lambda.Functions
{
    public class GetAllCategoriesLambda : LambdaBase
    {
        public async Task<APIGatewayProxyResponse> Handle(
            APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return await ExecuteAsync(request, context,
                async (serviceProvider, userContext) =>
                {
                    var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
                    var includeInactive = request.QueryStringParameters?.ContainsKey("includeInactive") == true 
                        && bool.Parse(request.QueryStringParameters["includeInactive"]);

                    var categories =  await categoryService.GetAllAsync(includeInactive);
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 201,
                        Body = JsonSerializer.Serialize(new
                        { categories = categories }),
                        Headers = new Dictionary<string, string>
                        {
                            ["Content-Type"] = "application/json"
                        }
                    };
                },
                requiresAuth: false);
        }
    }
}