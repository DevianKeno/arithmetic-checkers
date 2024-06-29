using System;

public static class EnumExtensions
{
    public static string ToSpacedString(this Enum value)
    {
        string enumName = value.ToString();
        return System.Text.RegularExpressions.Regex.Replace(enumName, "(\\B[A-Z])", " $1");
    }
}