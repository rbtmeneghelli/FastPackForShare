using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Bases;

public sealed class BaseResultPatternModel : GenericResultPatternModel
{
    public BaseResultPatternModel(int httpStatusCode, object data, string message): base(httpStatusCode, data, message)
    {
        
    }
}
