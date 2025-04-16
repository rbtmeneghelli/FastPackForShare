using FastPackForShare.Models;

namespace FastPackForShare.Interfaces.Factory;

public interface IExceptionErrorModelFactory
{
    ExceptionErrorModel GetResponseErrorModelByException(Exception exception);
}
