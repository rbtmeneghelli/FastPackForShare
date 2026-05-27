namespace FastPackForShare;

public sealed class CustomResponseModel
{
    public required int StatusCode { get; init; } = (int)HttpStatusCode.BadRequest;
    public required object Data { get; init; } = null;
    public required string Message { get; init; } = string.Empty;

    public CustomResponseModel()
    {
    }
}
