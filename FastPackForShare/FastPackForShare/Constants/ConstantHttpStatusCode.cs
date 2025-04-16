using System.Net;

namespace FastPackForShare.Constants;

public static class ConstantHttpStatusCode
{
    public const int OK_CODE = (int)HttpStatusCode.OK;
    public const int CREATE_CODE = (int)HttpStatusCode.Created;
    public const int NO_CONTENT_CODE = (int)HttpStatusCode.NoContent;
    public const int BAD_REQUEST_CODE = (int)HttpStatusCode.BadRequest;
    public const int UNAUTHORIZED_CODE = (int)HttpStatusCode.Unauthorized;
    public const int NOT_FOUND_CODE = (int)HttpStatusCode.NotFound;
    public const int NOT_ALLOWED_CODE = (int)HttpStatusCode.MethodNotAllowed;
    public const int AUTHENTICATION_REQUIRED_CODE = (int)HttpStatusCode.NetworkAuthenticationRequired;
    public const int INTERNAL_ERROR_CODE = (int)HttpStatusCode.InternalServerError;
    public const int FORBIDDEN_CODE = (int)HttpStatusCode.Forbidden;
    public const int UNPROCESSABLE_CONTENT = (int)HttpStatusCode.UnprocessableContent;
}
