using FastPackForShare.Bases.Generics;
using FastPackForShare.Default;
using FastPackForShare.Interfaces;
using FastPackForShare.Models;
using FluentValidation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FastPackForShare.Services.Bases;

public abstract class BaseHandlerService
{
    private readonly INotificationMessageService _iNotificationMessageService;

    protected BaseHandlerService(INotificationMessageService iNotificationMessageService)
    {
        _iNotificationMessageService = iNotificationMessageService;
    }

    protected void Notify(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            Notify(error.ErrorMessage);
        }
    }

    protected void Notify(string message)
    {
        _iNotificationMessageService.Handle(new NotificationMessageModel(message));
    }

    protected bool ExecuteValidation<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : GenericDTOModel
    {
        var validator = validacao.Validate(entidade);

        if (validator.IsValid) return true;

        Notify(validator);

        return false;
    }
}
