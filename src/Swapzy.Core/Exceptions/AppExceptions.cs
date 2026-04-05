namespace Swapzy.Core.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        protected AppException(string message, int statusCode, string errorCode)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, 404, "NOT_FOUND") { }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, 409, "CONFLICT") { }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            : base(message, 403, "FORBIDDEN") { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, 400, "BAD_REQUEST") { }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, 401, "UNAUTHORIZED") { }
    }
}
