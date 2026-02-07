using FastPackForShare.Constants;
using FastPackForShare.Extensions;

namespace FastPackForShare;

public sealed class CustomResponseModel
{
    public required int StatusCode { get; init; } = (int)HttpStatusCode.BadRequest;
    public required object Data { get; init; } = null;
    public required string Message { get; init; } = string.Empty;

    public CustomResponseModel()
    {
    }

    public CustomResponseModel(int statusCode)
    {
        StatusCode = statusCode;
        Data = default;
        Message = ConstantMessageResponse.GetMessageResponse(statusCode);
    }

    public CustomResponseModel(int statusCode, object data)
    {
        StatusCode = statusCode;
        Data = GuardClauseExtension.IsNotNull(data) ? data : null;
        Message = ConstantMessageResponse.GetMessageResponse(statusCode);
    }

    public CustomResponseModel(int statusCode, string message)
    {
        StatusCode = statusCode;
        Data = default;
        Message = message;
    }
}
