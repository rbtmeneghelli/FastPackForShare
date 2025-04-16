using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Default;

public sealed class BasePagedResultModel<T> : GenericPagedResultModel where T : class
{
    public IEnumerable<T> Results { get; set; }

    public BasePagedResultModel()
    {
        Results = Enumerable.Empty<T>();
    }
}
