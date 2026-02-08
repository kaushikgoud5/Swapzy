using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Swapzy.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.Interfaces
{
    public interface IAuthMiddleware
    {
        Task<UserContext> AuthenticateAsync(
            APIGatewayProxyRequest request,
            ILambdaContext context);
    }
}
