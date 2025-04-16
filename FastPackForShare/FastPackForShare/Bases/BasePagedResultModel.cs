using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Default;

public class BasePagedResultModel<T> : GenericPagedResultMOdel where T : class
{
    public IEnumerable<T> Results { get; set; }

    public BasePagedResultModel()
    {
        Results = Enumerable.Empty<T>();
    }
}
