namespace FastPackForShare.Interfaces.Factory;

public interface IExceptionErrorFactory
{
    IExceptionErrorModelFactory GetResponseErrorModelByException(Exception exception);
}
