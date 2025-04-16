using System.Diagnostics;
using FastPackForShare.Models;
using FastPackForShare.Constants;
using FastPackForShare.Interfaces.Factory;

namespace WbNotes.Application.Factory.ResponseErrorModel.Models;

public sealed class ApplicationExceptionModel : IExceptionErrorModelFactory
{
    public ApplicationExceptionModel()
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
            Title = "Server Error",
            ExceptionError = exception.Message
        };

        return exceptionErrorModel;
    }
}
