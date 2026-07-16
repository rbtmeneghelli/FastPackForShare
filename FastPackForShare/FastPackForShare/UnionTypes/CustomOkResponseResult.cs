namespace FastPackForShare.UnionTypes;

public record CustomOkResponseResult(int StatusCode, object Data = null, string Message = "")
{
    public static CustomOkResponseResult Create(int statusCode, object data, string message)
    {
        if (data is not null && !string.IsNullOrWhiteSpace(message))
            return new CustomOkResponseResult(statusCode, data, message);
        else if (data is not null && string.IsNullOrWhiteSpace(message))
            return new CustomOkResponseResult(statusCode, data);
        else if (data is null && !string.IsNullOrWhiteSpace(message))
            return new CustomOkResponseResult(statusCode, null, message);
        else
            return new CustomOkResponseResult(statusCode);
    }
}