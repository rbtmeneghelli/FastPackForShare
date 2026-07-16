using FastPackForShare.Constants;

namespace FastPackForShare.UnionTypes;

public static class CustomResponseResultFactory
{
    public static CustomResponseResult CreateResponse(int statusCode = 0, object data = null, string message = "")
    {
        return CustomResponseResult customResponseResult = statusCode switch
        {
            ConstantHttpStatusCode.OK_CODE or ConstantHttpStatusCode.CREATE_CODE => CustomOkResponseResult.Create(statusCode, data, message),
            ConstantHttpStatusCode.BAD_REQUEST_CODE => CustomErrorResponseResult.Create(statusCode, data, message),
            ConstantHttpStatusCode.NOT_FOUND_CODE => CustomErrorResponseResult.Create(statusCode, data, message),
            ConstantHttpStatusCode.INTERNAL_ERROR_CODE => CustomErrorResponseResult.Create(statusCode, data, message),
            _ => CustomErrorResponseResult.Create(statusCode, data, message)
        };
    }
}
