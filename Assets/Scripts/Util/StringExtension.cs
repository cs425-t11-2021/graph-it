using System.Globalization;

public static class StringExtension
{
    public static string ToTitleCase(this string s) =>
        CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
}
