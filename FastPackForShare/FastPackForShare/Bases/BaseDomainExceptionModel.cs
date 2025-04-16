namespace FastPackForShare.Default;

public sealed class BaseDomainException : Exception
{
    public BaseDomainException(string error) : base(error)
    {
    }

    public static void When(bool hasError, string error)
    {
        if (hasError)
            throw new BaseDomainException(error);
    }
}
