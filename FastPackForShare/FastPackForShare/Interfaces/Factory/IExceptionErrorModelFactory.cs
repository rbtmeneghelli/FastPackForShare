using FastPackForShare.Models;

namespace FastPackForShare.Interfaces.Factory;

public interface IExceptionErrorModelFactory
{
    ExceptionErrorModel GetResponseErrorModelByException(string traceId, Exception exception);
}
