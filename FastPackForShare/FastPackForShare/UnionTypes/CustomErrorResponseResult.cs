namespace FastPackForShare.UnionTypes;

public record CustomErrorResponseResult(int StatusCode, object Data = null, string Message = "")
{
    public static CustomErrorResponseResult Create(int statusCode, object data, string message)
    {
        if (data is not null && !string.IsNullOrWhiteSpace(message))
            return new CustomErrorResponseResult(statusCode, data, message);
        else if (data is not null && string.IsNullOrWhiteSpace(message))
            return new CustomErrorResponseResult(statusCode, data);
        else if (data is null && !string.IsNullOrWhiteSpace(message))
            return new CustomErrorResponseResult(statusCode, null, message);
        else
            return new CustomErrorResponseResult(statusCode);
    }
}
