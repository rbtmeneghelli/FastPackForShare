using System.Runtime.CompilerServices;

namespace FastPackForShare.Extensions;

public static class ValidatorExtension
{
    private const string ARGUMENTNULL_MESSAGE_ERROR = "O argumento não pode ser nulo ou vazio";
    private const string ARGUMENTOUT_MESSAGE_ERROR = "O argumento fornecido não é valido";

    public static void ThrowIfNull(object argument, [CallerArgumentExpression("argument")] string argumentExpression = null!)
    {
        if (GuardClauseExtension.IsNull(argument))
            throw new ArgumentNullException(argumentExpression, ARGUMENTNULL_MESSAGE_ERROR);
    }

    public static void ThrowIfNullOrEmpty(string argument, [CallerArgumentExpression("argument")] string argumentExpression = null!)
    {
        if (GuardClauseExtension.IsNullOrWhiteSpace(argument))
            throw new ArgumentNullException(argumentExpression, ARGUMENTNULL_MESSAGE_ERROR);
    }

    public static void ThrowIfPrimaryKeyNotValid(int argument, [CallerArgumentExpression("argument")] string argumentExpression = null!)
    {
        if (GuardClauseExtension.IsLessThanZeroOrEqual(argument))
            throw new ArgumentOutOfRangeException(argumentExpression, argument, ARGUMENTOUT_MESSAGE_ERROR);
    }

    public static void ThrowIfPrimaryKeyNotValid(int? argument, [CallerArgumentExpression("argument")] string argumentExpression = null!)
    {
        if (!argument.HasValue || GuardClauseExtension.IsLessThanZeroOrEqual(argument.Value))
            throw new ArgumentOutOfRangeException(argumentExpression, argument, ARGUMENTOUT_MESSAGE_ERROR);
    }

    public static void ThrowIfPrimaryKeyNotValid(long argument, [CallerArgumentExpression("argument")] string argumentExpression = null!)
    {
        if (GuardClauseExtension.IsLessThanZeroOrEqual(argument))
            throw new ArgumentOutOfRangeException(argumentExpression, argument, ARGUMENTOUT_MESSAGE_ERROR);
    }

    public static void ThrowIfPrimaryKeyNotValid(long? argument, [CallerArgumentExpression("argument")] string argumentExpression = null!)
    {
        if (!argument.HasValue || GuardClauseExtension.IsLessThanZeroOrEqual(argument.Value))
            throw new ArgumentOutOfRangeException(argumentExpression, argument, ARGUMENTOUT_MESSAGE_ERROR);
    }

    public static void ThrowIfOutOfRange<T>(T value, T min, T max, [CallerArgumentExpression("value")] string argumentExpression = null!)
    where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(argumentExpression, value, $"{ARGUMENTOUT_MESSAGE_ERROR}, o valor deve estar entre {min} e {max}.");
        }
    }

    public static void ThrowIfNotEqual<T>(T argument, T condition, [CallerArgumentExpression("argument")] string argumentExpression = null!)
    {
        if (argument.Equals(condition))
            throw new ArgumentOutOfRangeException(argumentExpression, argument, $"{ARGUMENTOUT_MESSAGE_ERROR} para a condição {condition}");
    }
}