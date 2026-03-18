using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace Swapzy.Lambda.Functions
{
    public interface IRouteHandler
    {
        bool RequiresAuth { get; }
        Task<APIGatewayProxyResponse> HandleAsync(
            APIGatewayProxyRequest request,
            ILambdaContext context);
    }
}
