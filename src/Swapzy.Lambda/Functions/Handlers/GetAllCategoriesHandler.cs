using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;

namespace Swapzy.Lambda.Functions.Handlers
{
    public class GetAllCategoriesHandler : LambdaBase, IRouteHandler
    {
        public bool RequiresAuth => false;

        public async Task<APIGatewayProxyResponse> HandleAsync(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            return await ExecuteAsync(request, context, async (sp, _) =>
            {
                var categoryService = sp.GetRequiredService<ICategoryService>();
                var includeInactive = request.QueryStringParameters?.ContainsKey("includeInactive") == true
                    && bool.Parse(request.QueryStringParameters["includeInactive"]);

                var categories = await categoryService.GetAllAsync(includeInactive);
                return Ok(new { categories });
            }, RequiresAuth);
        }
    }
}
