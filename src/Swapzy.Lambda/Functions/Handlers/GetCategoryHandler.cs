using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;

namespace Swapzy.Lambda.Functions.Handlers
{
    public class GetCategoryHandler : LambdaBase, IRouteHandler
    {
        public bool RequiresAuth => false;

        public async Task<APIGatewayProxyResponse> HandleAsync(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            return await ExecuteAsync(request, context, async (sp, _) =>
            {
                var categoryService = sp.GetRequiredService<ICategoryService>();

                if (request.PathParameters?.ContainsKey("id") == true)
                {
                    var id = int.Parse(request.PathParameters["id"]);
                    var cat = await categoryService.GetByIdAsync(id);
                    return cat != null ? Ok(new { category = cat }) : Error(404, "Category not found");
                }

                if (request.QueryStringParameters?.ContainsKey("slug") == true)
                {
                    var slug = request.QueryStringParameters["slug"];
                    var cat = await categoryService.GetBySlugAsync(slug);
                    return cat != null ? Ok(new { category = cat }) : Error(404, "Category not found");
                }

                return Error(400, "Category ID or slug is required");
            }, RequiresAuth);
        }
    }
}
