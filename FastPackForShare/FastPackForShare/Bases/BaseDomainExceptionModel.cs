namespace FastPackForShare.Default;

public sealed class BaseDomainException : Exception
{
    public BaseDomainException(string error) : base(error)
    {
    }

    public static void WhenIfNull(object source, string message)
    {
        ArgumentNullException.ThrowIfNull(source, message);
    }

    public static void WhenIsInvalid(bool hasError, string message)
    {
        if (hasError)
            throw new BaseDomainException(message);
    }
}
