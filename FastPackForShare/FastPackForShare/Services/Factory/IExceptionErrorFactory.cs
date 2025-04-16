namespace FastPackForShare.Services.Factory;

public interface IExceptionErrorFactory
{
    IExceptionErrorModelFactory GetResponseErrorModelByException(Exception exception);
}
