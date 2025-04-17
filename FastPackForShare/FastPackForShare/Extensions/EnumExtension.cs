using System.ComponentModel;
using System.Reflection;

namespace FastPackForShare.Extensions;

public static class EnumExtension
{
    public static string GetDisplayName(this Enum value)
    {
        return value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>(false)
            ?.Name
            ?? value.ToString();
    }

    public static string GetDisplayShortName(this Enum value)
    {
        return value
            .GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>(false)
            ?.ShortName
            ?? value.ToString();
    }

    public static T[] GetEnumValues<T>() where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("GetValues<T> can only be called for types derived from System.Enum", "T");

        return (T[])Enum.GetValues(typeof(T));
    }

    public static string GetEnumDescription(this Enum value)
    {
        DescriptionAttribute attribute = value.GetType()
            .GetField(value.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .SingleOrDefault() as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
}
