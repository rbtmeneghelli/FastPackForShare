using FastPackForShare.Constants;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;

namespace FastPackForShare.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
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
        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Ocorreu um erro inesperado do servidor",
            Status = MapExceptionToStatusCode(exception),
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static int MapExceptionToStatusCode(Exception ex) =>
    ex switch
    {
        ArgumentException or ArgumentNullException or FormatException or
        InvalidCastException or KeyNotFoundException or ValidationException or
        ValidationException or BadRequestException or Newtonsoft.Json.JsonException or
        JsonReaderException
        => ConstantHttpStatusCode.BAD_REQUEST_CODE,
        NullReferenceException or InvalidOperationException or IndexOutOfRangeException or DivideByZeroException or
        IOException or FileNotFoundException or TimeoutException or OutOfMemoryException or
        StackOverflowException or TaskCanceledException
        => ConstantHttpStatusCode.INTERNAL_ERROR_CODE,
        UnauthorizedAccessException or ForbiddenException
        => ConstantHttpStatusCode.UNAUTHORIZED_CODE,
        NotFoundException
        => ConstantHttpStatusCode.NOT_FOUND_CODE,
        _ => ConstantHttpStatusCode.INTERNAL_ERROR_CODE
    };
}
