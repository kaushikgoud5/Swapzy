using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Common;

namespace Swapzy.Lambda.Functions.Base
{
    public abstract class LambdaBase
    {
        private static readonly IServiceProvider _serviceProvider;
        static LambdaBase()
        {
            var configuration = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
                    .AddEnvironmentVariables()
                    .Build();

            var services = new ServiceCollection();

            ServiceConfigurations.Register(services, configuration);

            _serviceProvider = services.BuildServiceProvider();
        }
        protected async Task<TResponse> ExecuteAsync<TResponse>(
            APIGatewayProxyRequest request,
            ILambdaContext context,
            Func<IServiceProvider, UserContext?, Task<TResponse>> action,
            bool requiresAuth = true)
        {
            using var scope = _serviceProvider.CreateScope();
            try
            {
                UserContext? userContext = null;

                if (requiresAuth)
                {
                    var auth = scope.ServiceProvider.GetRequiredService<IAuthMiddleware>();
                    userContext = await auth.AuthenticateAsync(request, context);
                }

                return await action(scope.ServiceProvider, userContext);
            }
            catch (UnauthorizedAccessException)
            {
                return (TResponse)(object)new APIGatewayProxyResponse
                {
                    StatusCode = 401,
                    Body = "Unauthorized"
                };
            }
        }

    }
}
