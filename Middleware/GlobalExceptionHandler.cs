using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using taskmanager_api.Exceptions;

namespace taskmanager_api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        string title;

        if (exception is AppException appException)
        {
            // Expected error: safe to show the message to the client
            statusCode = appException.StatusCode;
            title = appException.Message;
            _logger.LogWarning("Handled error {StatusCode}: {Message}", statusCode, title);
        }
        else
        {
            // Unexpected error: log everything, tell the client nothing
            statusCode = StatusCodes.Status500InternalServerError;
            title = "An unexpected error occurred.";
            _logger.LogError(exception, "Unhandled exception");
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = statusCode, Title = title },
            cancellationToken);

        return true; // true = "I handled it, stop here"
    }
}