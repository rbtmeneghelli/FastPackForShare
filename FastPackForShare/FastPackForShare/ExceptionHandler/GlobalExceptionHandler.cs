using System.Diagnostics;
using System.Web.Http.ExceptionHandling;
using FastPackForShare.Models;
using Microsoft.AspNetCore.Http;

namespace FastPackForShare.ExceptionHandler;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IExceptionErrorModelFactory _iExceptionErrorModelFactory;
    private readonly ISeriLogService _iSeriLogService;

    public GlobalExceptionHandler(IExceptionErrorModelFactory iExceptionErrorModelFactory, ISeriLogService iSeriLogService)
    {
        _iExceptionErrorModelFactory = iExceptionErrorModelFactory;
        _iSeriLogService = iSeriLogService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var logService = httpContext.RequestServices.GetService<ILogService>();

        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
        IExceptionErrorModelConfigFactory exceptionErrorModelConfigFactory = _iExceptionErrorModelFactory.GetResponseErrorModelByException(exception);
        ExceptionErrorModel exceptionErrorModel = exceptionErrorModelConfigFactory.GetResponseErrorModelByException(logService, _iSeriLogService, exception);

        httpContext.Response.StatusCode = exceptionErrorModel.StatusCode;

        await httpContext.Response.WriteAsJsonAsync(exceptionErrorModel, cancellationToken);

        return true;
    }
}