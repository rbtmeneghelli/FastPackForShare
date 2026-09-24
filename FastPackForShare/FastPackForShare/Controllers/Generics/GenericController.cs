using FastPackForShare.Constants;
using FastPackForShare.Extensions;
using FastPackForShare.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using FastPackForShare.Interfaces;
using FastPackForShare.Bases;
using Microsoft.AspNetCore.Http;

namespace FastPackForShare.Controllers.Generics;

[ApiController]
[Produces("application/json")]
[ProducesResponseType(ConstantHttpStatusCode.NO_CONTENT_CODE, Type = typeof(CustomValidResponseTypeModel<object>))]
[ProducesResponseType(ConstantHttpStatusCode.BAD_REQUEST_CODE, Type = typeof(CustomInValidResponseTypeModel))]
[ProducesResponseType(ConstantHttpStatusCode.INTERNAL_ERROR_CODE, Type = typeof(CustomInValidResponseTypeModel))]
[ProducesResponseType(ConstantHttpStatusCode.FORBIDDEN_CODE, Type = typeof(CustomInValidResponseTypeModel))]
[ProducesResponseType(ConstantHttpStatusCode.INTERNAL_ERROR_CODE, Type = typeof(CustomInValidResponseTypeModel))]
[RequestSizeLimit(2 * 1024 * 1024)]
[ValidateAntiForgeryToken]
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

    public IResult CustomResponseModel(ModelStateDictionary modelState)
    {
        NotificationModelIsInvalid(modelState);
        return CustomResponse(ConstantHttpStatusCode.BAD_REQUEST_CODE);
    }

    protected IResult CustomResponse(int statusCode = ConstantHttpStatusCode.OK_CODE, object result = null, string messageResponse = "")
    {
        var response = OperationIsValid() ?
                       new CustomResponseModel
                       (
                           statusCode,
                           result,
                           statusCode == ConstantHttpStatusCode.CREATE_CODE
                         ? ConstantMessageResponse.CREATE_CODE
                         : messageResponse
                       ) :
                       new CustomResponseModel
                       (
                           statusCode,
                           null,
                           _notificationService.HaveNotification()
                          ? ConstantMessageResponse.GetMessageResponse(statusCode)
                          : string.Join(',', _notificationService.GetNotifications().Select(n => n.Message))
                       );

        return GetResultFromStatusCode(statusCode, response);
    }

    protected IResult CustomResponse(CustomResponseModel customResponseModel)
    {
        var response = OperationIsValid() ?
                       new CustomResponseModel
                       (
                           customResponseModel.StatusCode,
                           customResponseModel.Data,
                           customResponseModel.StatusCode == ConstantHttpStatusCode.CREATE_CODE
                         ? ConstantMessageResponse.CREATE_CODE
                         : customResponseModel.Message
                       ) :
                       new CustomResponseModel
                       (
                           customResponseModel.StatusCode,
                           null,
                           _notificationService.HaveNotification()
                          ? ConstantMessageResponse.GetMessageResponse(customResponseModel.StatusCode)
                          : string.Join(',', _notificationService.GetNotifications().Select(n => n.Message))
                       );

        return GetResultFromStatusCode(customResponseModel.StatusCode, response);
    }

    private IResult GetResultFromStatusCode(int statusCode, CustomResponseModel response)
    {
        return statusCode switch
        {
            ConstantHttpStatusCode.OK_CODE => Results.Ok(response),
            ConstantHttpStatusCode.CREATE_CODE => Results.Created(),
            ConstantHttpStatusCode.BAD_REQUEST_CODE | ConstantHttpStatusCode.UNPROCESSABLE_CONTENT => Results.BadRequest(response),
            ConstantHttpStatusCode.UNAUTHORIZED_CODE | ConstantHttpStatusCode.AUTHENTICATION_REQUIRED_CODE => Results.Unauthorized(),
            ConstantHttpStatusCode.NOT_FOUND_CODE => Results.NotFound(response),
            ConstantHttpStatusCode.INTERNAL_ERROR_CODE | ConstantHttpStatusCode.SERVICE_UNAVAILABLE_CODE | ConstantHttpStatusCode.BAD_GATEWAY_CODE => Results.InternalServerError(response),
            ConstantHttpStatusCode.FORBIDDEN_CODE => Results.Forbid(),

            _ => Results.NoContent()
        };
    }
}