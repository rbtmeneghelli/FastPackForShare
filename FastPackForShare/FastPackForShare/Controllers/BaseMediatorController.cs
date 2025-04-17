using FastPackForShare.Controllers.Generics;
using FastPackForShare.Interfaces;
using MediatR;

namespace FastPackForShare.Controllers;

public abstract class BaseMediatorController : GenericController
{
    protected readonly IMediator _iMediator;

    public BaseMediatorController(IMediator iMediator, INotificationMessageService notificationMessageService) :
    base(notificationMessageService)
    {
        _iMediator = iMediator;
    }
}
