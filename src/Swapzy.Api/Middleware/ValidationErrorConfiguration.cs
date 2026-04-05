using Microsoft.AspNetCore.Mvc;

namespace Swapzy.Api.Middleware
{
    public static class ValidationErrorConfiguration
    {
        public static IMvcBuilder ConfigureValidationErrors(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            e => e.Key,
                            e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                        );

                    var response = new ApiErrorResponse
                    {
                        Status = 422,
                        ErrorCode = "VALIDATION_ERROR",
                        Message = "One or more validation errors occurred.",
                        TraceId = context.HttpContext.TraceIdentifier,
                        Errors = errors
                    };

                    return new UnprocessableEntityObjectResult(response);
                };
            });

            return builder;
        }
    }
}
