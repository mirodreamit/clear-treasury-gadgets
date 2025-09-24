namespace CT.Application.Abstractions.Extensions;

public static class StringExtensions
{
    public static string FirstCharToLowerInvariant(this string str)
    {
        return str[..1].ToLowerInvariant() + str[1..];
    }

    public static string FirstCharToUpperInvariant(this string str)
    {
        return str[..1].ToUpperInvariant() + str[1..];
    }

    public static DateOnly? ToDateOnly(this string str)
    {
        if (str == null)
        {
            return null;
        }

        return DateOnly.Parse(str);
    }
}
