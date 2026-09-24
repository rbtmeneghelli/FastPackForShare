namespace FastPackForShare;

public sealed class CustomResponseModel
{
    public int StatusCode { get; init; } = (int)HttpStatusCode.BadRequest;
    public object Data { get; init; } = null;
    public string Message { get; init; } = string.Empty;

    public CustomResponseModel()
    {
    }

    public CustomResponseModel(int statusCode, object data, string message)
    {
        StatusCode = statusCode;
        Data = data;
        Message = message;
    }
}
