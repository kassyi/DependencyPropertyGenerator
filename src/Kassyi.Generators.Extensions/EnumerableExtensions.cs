namespace Kassyi.Generators.Extensions;

/// <summary>Provides extension methods for <see cref="IEnumerable{T}"/> collections.</summary>
public static class EnumerableExtensions
{
    /// <summary>Concatenates strings and trims leading/trailing line breaks, returning a single space if empty.</summary>
    public static string Inject(this IEnumerable<string> values)
    {
        var text = string.Concat(values)
            .TrimStart('\r', '\n')
            .TrimEnd('\r', '\n');
        return string.IsNullOrWhiteSpace(text) ? " " : text;
    }
}
