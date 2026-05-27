using FastPackForShare.Constants;
using FastPackForShare.Extensions;
using FastPackForShare.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using FastPackForShare.Interfaces;
using FastPackForShare.Bases;

namespace FastPackForShare.Controllers.Generics;

[ApiController]
[Produces("application/json")]
[ProducesResponseType(ConstantHttpStatusCode.NO_CONTENT_CODE, Type = typeof(CustomValidResponseTypeModel<object>))]
[ProducesResponseType(ConstantHttpStatusCode.BAD_REQUEST_CODE, Type = typeof(CustomInValidResponseTypeModel))]
[ProducesResponseType(ConstantHttpStatusCode.INTERNAL_ERROR_CODE, Type = typeof(CustomInValidResponseTypeModel))]
[ProducesResponseType(ConstantHttpStatusCode.FORBIDDEN_CODE, Type = typeof(CustomInValidResponseTypeModel))]
[ProducesResponseType(ConstantHttpStatusCode.INTERNAL_ERROR_CODE, Type = typeof(CustomInValidResponseTypeModel))]
public abstract class GenericController : ControllerBase
{
    protected readonly INotificationMessageService _notificationService;

    protected int HttpCodeStatus { get; set; }

    protected GenericController(INotificationMessageService notificationService)
    {
        HttpCodeStatus = ConstantHttpStatusCode.BAD_REQUEST_CODE;
        _notificationService = notificationService;
    }

    protected bool ModelStateIsInvalid()
    {
        return ModelState.IsValid ? false : true;
    }

    protected void NotificationError(string mensagem)
    {
        _notificationService.Handle(new NotificationMessageModel(mensagem));
    }

    protected bool OperationIsValid()
    {
        return !_notificationService.HaveNotification();
    }

    private void NotificationModelIsInvalid(ModelStateDictionary modelState)
    {
        var erros = modelState.Values.SelectMany(e => e.Errors);
        foreach (var erro in erros)
        {
            var errorMsg = GuardClauseExtension.IsNull(erro.Exception) ? erro.ErrorMessage : erro.Exception.Message;
            NotificationError(errorMsg);
        }
    }

    public IActionResult CustomResponseModel(ModelStateDictionary modelState)
    {
        NotificationModelIsInvalid(modelState);
        return CustomResponse(ConstantHttpStatusCode.BAD_REQUEST_CODE);
    }

    public IActionResult CustomResponse(int statusCode = ConstantHttpStatusCode.OK_CODE, object result = null, string messageResponse = "")
    {
        int[] arrStatusCode = [ConstantHttpStatusCode.OK_CODE, ConstantHttpStatusCode.CREATE_CODE];
        var message = string.Empty;

        if (OperationIsValid() && arrStatusCode.Contains(statusCode))
        {
            message = result is not null ? 
                      messageResponse : 
                      ConstantMessageResponse.NO_CONTENT_CODE;
        }
        else
        {
            message = _notificationService.HaveNotification()
                      ? ConstantMessageResponse.GetMessageResponse(statusCode)
                      : string.Join(',', _notificationService.GetNotifications().Select(n => n.Message));
        }

        return StatusCode(statusCode, new BaseResultPatternModel(statusCode, result, message));
    }

    public IActionResult CustomResponse(CustomResponseModel customResponseModel)
    {
        int[] arrStatusCode = [ConstantHttpStatusCode.OK_CODE, ConstantHttpStatusCode.CREATE_CODE];
        var message = string.Empty;

        if (OperationIsValid() && arrStatusCode.Contains(customResponseModel.StatusCode))
        {
            message = customResponseModel.Data is not null ?
                      customResponseModel.Message :
                      ConstantMessageResponse.NO_CONTENT_CODE;
        }
        else
        {
            message = _notificationService.HaveNotification() ?
                      string.Join(',', _notificationService.GetNotifications().Select(n => n.Message)) :
                      string.Empty;
        }

        return StatusCode(customResponseModel.StatusCode, new BaseResultPatternModel(customResponseModel.StatusCode, customResponseModel.Data, message));
    }
}
