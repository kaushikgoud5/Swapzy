using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application;
using Swapzy.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Lambda
{
    static class ServiceConfigurations
    {
        public static void Register(ServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI(configuration);
        }
    }
}
