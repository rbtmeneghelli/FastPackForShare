using FastPackForShare.Default;
using FastPackForShare.Exceptions;
using FastPackForShare.Interfaces.Factory;
using Microsoft.IdentityModel.Tokens;
using WbNotes.Application.Factory.ResponseErrorModel.Models;

namespace FastPackForShare.Services.Factory;

public sealed class ExceptionErrorFactory : IExceptionErrorFactory
{
    public IExceptionErrorModelFactory GetResponseErrorModelByException(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => new ArgumentNullExceptionModel(),
            ApplicationException => new ApplicationExceptionModel(),
            UnauthorizedAccessException => new UnauthorizedAccessExceptionModel(),
            RegexMatchTimeoutException => new RegexMatchTimeoutExceptionModel(),
            BaseDomainException => new BaseDomainExceptionModel(),
            SecurityTokenException => new SecurityTokenExceptionModel(),
            _ => new GenericExceptionModel()
        };
    }
}
