using FastPackForShare.Constants;
using FastPackForShare.Extensions;

namespace FastPackForShare;

public sealed class CustomResponseModel
{
    public int StatusCode { get; init; }
    public object Data { get; init; }
    public string Message { get; init; }

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
