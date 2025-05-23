﻿using FastPackForShare.Models;
using FastPackForShare.Constants;
using FastPackForShare.Interfaces.Factory;

namespace WbNotes.Application.Factory.ResponseErrorModel.Models;

public sealed class UnauthorizedAccessExceptionModel : IExceptionErrorModelFactory
{
    public UnauthorizedAccessExceptionModel()
    {
    }

    public ExceptionErrorModel GetResponseErrorModelByException(Exception exception)
    {
        StackTrace stackTrace = new StackTrace(exception, true);
        StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);

        ExceptionErrorModel exceptionErrorModel = new()
        {
            StatusCode = ConstantHttpStatusCode.AUTHENTICATION_REQUIRED_CODE,
            Success = false,
            Title = "Token Expired",
            ExceptionError = exception.Message
        };

        return exceptionErrorModel;
    }
}