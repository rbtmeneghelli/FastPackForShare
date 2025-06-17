using FastPackForShare.Default;

namespace FastPackForShare.Bases;

public class BaseEntityComparer<TBaseEntityModel> : IEqualityComparer<TBaseEntityModel> where TBaseEntityModel : BaseEntityModel
{
    public bool Equals(TBaseEntityModel x, TBaseEntityModel y)
    {
        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(TBaseEntityModel obj)
    {
        return obj.Id.GetHashCode();
    }
}
