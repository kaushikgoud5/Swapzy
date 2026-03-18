using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Swapzy.Lambda.Functions.Handlers;
using System.Text.Json;
using System.Text.RegularExpressions;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Swapzy.Lambda.Functions
{
    public class ApiRouter
    {
        private readonly List<RouteEntry> _routes;

        private static readonly Dictionary<string, string> CorsHeaders = new()
        {
            ["Content-Type"] = "application/json",
            ["Access-Control-Allow-Origin"] = "*",
            ["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS",
            ["Access-Control-Allow-Headers"] = "Content-Type,Authorization"
        };

        public ApiRouter()
        {
            _routes =
            [
                new("POST", "/auth/register", new RegisterUserHandler()),
                new("POST", "/auth/login", new LoginUserHandler()),
                new("GET", "/categories", new GetAllCategoriesHandler()),
                new("POST", "/categories", new CreateCategoryHandler()),
                new("GET", "/categories/{id}", new GetCategoryHandler()),
            ];
        }

        public async Task<APIGatewayProxyResponse> Handle(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            var rawJson = JsonSerializer.Serialize(request);
            context.Logger.LogInformation($"Raw request: {rawJson}");

            var method = request.HttpMethod ?? request.RequestContext?.HttpMethod;
            var path = request.Path ?? request.Resource;

            // Strip stage prefix (e.g. /prod/auth/register -> /auth/register)
            var stage = request.RequestContext?.Stage;
            if (!string.IsNullOrEmpty(stage) && path?.StartsWith($"/{stage}") == true)
                path = path.Substring(stage.Length + 1);

            context.Logger.LogInformation($"Incoming: Method={method}, Path={path}, Resource={request.Resource}");

            // Handle CORS preflight
            if (method == "OPTIONS")
                return new APIGatewayProxyResponse { StatusCode = 200, Headers = CorsHeaders };

            try
            {
                var matched = MatchRoute(method, path);
                if (matched == null)
                {
                    context.Logger.LogWarning($"No route matched: {method} {path}");
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 404,
                        Body = JsonSerializer.Serialize(new { error = "Route not found" }),
                        Headers = CorsHeaders
                    };
                }

                // Inject extracted path parameters
                request.PathParameters ??= new Dictionary<string, string>();
                foreach (var param in matched.Value.PathParams)
                    request.PathParameters[param.Key] = param.Value;

                context.Logger.LogInformation($"Routing: {method} {path} -> {matched.Value.Handler.GetType().Name}");
                return await matched.Value.Handler.HandleAsync(request, context);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Router error: {ex}");
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = JsonSerializer.Serialize(new { error = ex.ToString() }),
                    Headers = CorsHeaders
                };
            }
        }

        private (IRouteHandler Handler, Dictionary<string, string> PathParams)? MatchRoute(string? method, string? path)
        {
            if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(path))
                return null;

            path = path.TrimEnd('/');

            foreach (var route in _routes)
            {
                if (!string.Equals(route.Method, method, StringComparison.OrdinalIgnoreCase))
                    continue;

                var match = route.Pattern.Match(path);
                if (!match.Success) continue;

                var pathParams = new Dictionary<string, string>();
                foreach (var paramName in route.ParamNames)
                    pathParams[paramName] = match.Groups[paramName].Value;

                return (route.Handler, pathParams);
            }

            return null;
        }

        private class RouteEntry
        {
            public string Method { get; }
            public Regex Pattern { get; }
            public List<string> ParamNames { get; }
            public IRouteHandler Handler { get; }

            public RouteEntry(string method, string template, IRouteHandler handler)
            {
                Method = method;
                Handler = handler;
                ParamNames = [];

                // Convert /categories/{id} -> /categories/(?<id>[^/]+)
                var pattern = Regex.Replace(template, @"\{(\w+)\}", m =>
                {
                    ParamNames.Add(m.Groups[1].Value);
                    return $"(?<{m.Groups[1].Value}>[^/]+)";
                });

                Pattern = new Regex($"^{pattern}$", RegexOptions.Compiled);
            }
        }
    }
}
