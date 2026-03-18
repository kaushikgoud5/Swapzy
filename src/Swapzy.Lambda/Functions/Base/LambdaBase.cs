using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Common;
using System.Text.Json;

namespace Swapzy.Lambda.Functions.Base
{
    public abstract class LambdaBase
    {
        private static readonly IServiceProvider _serviceProvider;
        protected static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private static readonly Dictionary<string, string> CorsHeaders = new()
        {
            ["Content-Type"] = "application/json",
            ["Access-Control-Allow-Origin"] = "*",
            ["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS",
            ["Access-Control-Allow-Headers"] = "Content-Type,Authorization"
        };

        static LambdaBase()
        {
            var basePath = Path.GetDirectoryName(typeof(LambdaBase).Assembly.Location) ?? Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            ServiceConfigurations.Register(services, configuration);
            _serviceProvider = services.BuildServiceProvider();
        }

        protected async Task<APIGatewayProxyResponse> ExecuteAsync(
            APIGatewayProxyRequest request,
            ILambdaContext context,
            Func<IServiceProvider, UserContext?, Task<APIGatewayProxyResponse>> action,
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
            catch (UnauthorizedAccessException ex)
            {
                context.Logger.LogWarning($"Unauthorized: {ex.Message}");
                return Error(401, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                context.Logger.LogWarning($"Bad request: {ex.Message}");
                return Error(400, ex.Message);
            }
            catch (ArgumentException ex)
            {
                context.Logger.LogWarning($"Invalid argument: {ex.Message}");
                return Error(400, ex.Message);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Unhandled error: {ex}");
                return Error(500, ex.ToString());
            }
        }

        protected static APIGatewayProxyResponse Ok(object data, int statusCode = 200) => new()
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(data),
            Headers = CorsHeaders
        };

        protected static APIGatewayProxyResponse Error(int statusCode, string message) => new()
        {
            StatusCode = statusCode,
            Body = JsonSerializer.Serialize(new { error = message }),
            Headers = CorsHeaders
        };

        protected static T Deserialize<T>(string body) =>
            JsonSerializer.Deserialize<T>(body, JsonOptions)
            ?? throw new ArgumentException($"Invalid request body for {typeof(T).Name}");
    }
}
