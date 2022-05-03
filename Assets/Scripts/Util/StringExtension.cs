using System.Globalization;

// Extension for the C# string class that adds a ToTitleCase feature.
public static class StringExtension
{
    public static string ToTitleCase(this string s) =>
        CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
}
