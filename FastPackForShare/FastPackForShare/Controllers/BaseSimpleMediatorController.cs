using FastPackForShare.Controllers.Generics;
using FastPackForShare.Interfaces;
using FastPackForShare.SimpleMediator;

namespace FastPackForShare.Controllers;

public abstract class BaseSimpleMediatorController : GenericController
{
    // AQUI SERA IMPLEMENTADO O MEDIATOR BASEADO NO QUE O EDUARDO PIRES FEZ!!!
    protected readonly IMediator _iMediator;

    public BaseSimpleMediatorController(IMediator iMediator, INotificationMessageService notificationMessageService) :
    base(notificationMessageService)
    {
        _iMediator = iMediator;
    }
}
