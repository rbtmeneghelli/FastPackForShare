using System.Diagnostics;
using FastPackForShare.Models;
using FastPackForShare.Constants;
using FastPackForShare.Services.Factory;

namespace FastPackForShare.Exceptions;

public sealed class SecurityTokenExceptionModel : IExceptionErrorModelFactory
{
    public SecurityTokenExceptionModel()
    {
    }

    public ExceptionErrorModel GetResponseErrorModelByException(Exception exception)
    {
        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);

        ExceptionErrorModel exceptionErrorModel = new()
        {
            StatusCode = ConstantHttpStatusCode.UNAUTHORIZED_CODE,
            Success = false,
            Title = "Token Invalid",
            ExceptionError = exception.Message
        };

        return exceptionErrorModel;
    }
}
}
