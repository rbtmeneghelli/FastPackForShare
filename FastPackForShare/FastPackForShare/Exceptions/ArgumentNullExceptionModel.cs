using System.Diagnostics;
using FastPackForShare.Models;
using FastPackForShare.Constants;
using FastPackForShare.Services.Factory;

namespace WbNotes.Application.Factory.ResponseErrorModel.Models;

public sealed class ArgumentNullExceptionModel : IExceptionErrorModelFactory
{
    public ArgumentNullExceptionModel()
    {
    }

    public ExceptionErrorModel GetResponseErrorModelByException(Exception exception)
    {
        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);

        ExceptionErrorModel exceptionErrorModel = new()
        {
            StatusCode = ConstantHttpStatusCode.BAD_REQUEST_CODE,
            Success = false,
            Title = "Bad Request",
            ExceptionError = exception.Message
        };

        return exceptionErrorModel;
    }
}
