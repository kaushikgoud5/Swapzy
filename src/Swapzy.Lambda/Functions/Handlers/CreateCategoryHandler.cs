using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.Interfaces;
using Swapzy.Lambda.Functions.Base;

namespace Swapzy.Lambda.Functions.Handlers
{
    public class CreateCategoryHandler : LambdaBase, IRouteHandler
    {
        public bool RequiresAuth => true;

        public async Task<APIGatewayProxyResponse> HandleAsync(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            return await ExecuteAsync(request, context, async (sp, userContext) =>
            {
                var categoryService = sp.GetRequiredService<ICategoryService>();
                var dto = Deserialize<CreateCategoryDto>(request.Body);
                var result = await categoryService.CreateAsync(dto, userContext!.UserId);
                return Ok(new { categoryId = result.Id, category = result }, 201);
            }, RequiresAuth);
        }
    }
}
