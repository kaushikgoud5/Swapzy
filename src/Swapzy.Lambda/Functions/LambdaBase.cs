using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Swapzy.Lambda.Functions
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
            Func<IServiceProvider, Task<TResponse>> action,
            ILambdaContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            try
            {
                return await action(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
