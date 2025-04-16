using FastPackForShare.Models;

namespace FastPackForShare.Services.Factory;

public interface IExceptionErrorModelFactory
{
    ExceptionErrorModel GetResponseErrorModelByException(Exception exception);
}
