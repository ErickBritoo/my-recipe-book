using System.Diagnostics.CodeAnalysis;

namespace MyRecipeBook.Domain.Extensions;

public static class StringExtension
{
    public static bool IsNotEmpty([NotNullWhen(true)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value) == false;
    }
}