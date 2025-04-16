using System.Diagnostics;
using FastPackForShare.Models;
using FastPackForShare.Constants;
using FastPackForShare.Services.Factory;

namespace FastPackForShare.Exceptions;

public sealed class BaseDomainExceptionModel : IExceptionErrorModelFactory
{
    public BaseDomainExceptionModel()
    {
    }

    public ExceptionErrorModel GetResponseErrorModelByException(Exception exception)
    {
        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);

        ExceptionErrorModel exceptionErrorModel = new()
        {
            StatusCode = ConstantHttpStatusCode.INTERNAL_ERROR_CODE,
            Success = false,
            Title = "Domain from entity or model Error",
            ExceptionError = exception.Message
        };

        return exceptionErrorModel;
    }
}
