using FastPackForShare.Controllers.Generics;
using FastPackForShare.Interfaces;
using FastPackForShare.SimpleMediator;

namespace FastPackForShare.Controllers;

public abstract class BaseSimpleMediatorController : GenericController
{
    public readonly IMediator _iMediator;

    public BaseSimpleMediatorController(IMediator iMediator, INotificationMessageService notificationMessageService) :
    base(notificationMessageService)
    {
        _iMediator = iMediator;
    }
}
