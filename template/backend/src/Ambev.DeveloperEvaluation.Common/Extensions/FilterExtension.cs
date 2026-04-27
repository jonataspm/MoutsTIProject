namespace Ambev.DeveloperEvaluation.Common.Extensions;

public static class FilterExtension
{
    public static string CapitalizeFirst(this string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return s;
        s = s.ToLower();
        return char.ToUpper(s[0]) + s.Substring(1).ToLower();
    }
}
