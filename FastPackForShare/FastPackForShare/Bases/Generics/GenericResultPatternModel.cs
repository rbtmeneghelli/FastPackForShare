using FastPackForShare.Constants;
using FastPackForShare.Extensions;

namespace FastPackForShare.Bases.Generics;

public abstract class GenericResultPatternModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public object Value { get; set; }

    public GenericResultPatternModel(int httpStatusCode, object value = null, string message = "")
    {
        IsSuccess = ConstantHttpStatusCode.TransformHttpStatusCodeToBool(httpStatusCode);
        Message = httpStatusCode.Equals(ConstantHttpStatusCode.OK_CODE) ? message :
                  httpStatusCode.Equals(ConstantHttpStatusCode.OK_CODE) == false && GuardClauseExtension.IsNullOrWhiteSpace(message) ? ConstantMessageResponse.GetMessageResponse(httpStatusCode) :
                  message;
        Value = value;
    }
}
