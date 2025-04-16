namespace FastPackForShare.Default;

public class BaseDomainExceptionModel : Exception
{
    public BaseDomainExceptionModel(string error) : base(error)
    {
    }

    public static void When(bool hasError, string error)
    {
        if (hasError)
            throw new BaseDomainExceptionModel(error);
    }
}
