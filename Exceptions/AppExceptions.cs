namespace taskmanager_api.Exceptions;

// Base class: every "expected" error carries the HTTP status it maps to
public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, StatusCodes.Status404NotFound) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, StatusCodes.Status409Conflict) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message)
        : base(message, StatusCodes.Status401Unauthorized) { }
}