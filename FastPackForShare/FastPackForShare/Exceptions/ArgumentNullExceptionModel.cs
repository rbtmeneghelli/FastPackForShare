using FastPackForShare.Constants;
using FastPackForShare.Interfaces.Factory;
using FastPackForShare.Models;

namespace WbNotes.Application.Factory.ResponseErrorModel.Models;

public sealed class ArgumentNullExceptionModel : IExceptionErrorModelFactory
{
    public ArgumentNullExceptionModel()
    {
    }

    public ExceptionErrorModel GetResponseErrorModelByException(string traceId, Exception exception)
    {
        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);

        return new ExceptionErrorModel
        {
            Status = ConstantHttpStatusCode.BAD_REQUEST_CODE,
            Type = "Bad Request",
            Title = "API Bad Request",
            Detail = string.Format(ConstantMessageResponse.BAD_REQUEST_CODE_EXCEPTION, traceId),
        };
    }
}
