using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;
using System.Text.Json;

namespace Swapzy.Lambda.Functions
{
    public class GetCategoryLambda : LambdaBase
    {
        public async Task<APIGatewayProxyResponse> Handle(
            APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return await ExecuteAsync(request, context,
                async (serviceProvider, userContext) =>
                {
                    var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
                    
                    if (request.PathParameters?.ContainsKey("id") == true)
                    {
                        var id = int.Parse(request.PathParameters["id"]);
                        var cat = await categoryService.GetByIdAsync(id);
                        return new APIGatewayProxyResponse
                        {
                            StatusCode = 201,
                            Body = JsonSerializer.Serialize(new
                            { category = cat }),
                            Headers = new Dictionary<string, string>
                            {
                                ["Content-Type"] = "application/json"
                            }
                        };
                    }
                    else if (request.QueryStringParameters?.ContainsKey("slug") == true)
                    {
                        var slug = request.QueryStringParameters["slug"];
                        var cat =  await categoryService.GetBySlugAsync(slug);
                        return new APIGatewayProxyResponse
                        {
                            StatusCode = 201,
                            Body = JsonSerializer.Serialize(new
                            { category = cat }),
                            Headers = new Dictionary<string, string>
                            {
                                ["Content-Type"] = "application/json"
                            }
                        };
                    }

                    throw new ArgumentException("Category ID or slug is required");
                },
                requiresAuth: false);
        }
    }
}