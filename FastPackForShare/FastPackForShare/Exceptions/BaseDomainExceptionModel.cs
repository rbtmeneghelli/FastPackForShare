using FastPackForShare.Constants;
using FastPackForShare.Interfaces.Factory;
using FastPackForShare.Models;

namespace FastPackForShare.Exceptions;

public sealed class BaseDomainExceptionModel : IExceptionErrorModelFactory
{
    public BaseDomainExceptionModel()
    {
    }

    public ExceptionErrorModel GetResponseErrorModelByException(string traceId, Exception exception)
    {
        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);

        return new ExceptionErrorModel
        {
            Status = ConstantHttpStatusCode.INTERNAL_ERROR_CODE,
            Type = "Internal Error",
            Title = "API Internal Error",
            Detail = string.Format(ConstantMessageResponse.INTERNAL_ERROR_CODE_EXCEPTION, traceId),
        };
    }
}
