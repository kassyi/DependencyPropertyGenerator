using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides extension methods for querying <see cref="AttributeData"/> arguments.</summary>
public static class AttributeDataExtensions
{
    /// <summary>Returns the generic type argument at the specified index, or <see langword="null"/> if not found.</summary>
    public static ITypeSymbol? GetGenericTypeArgument(this AttributeData attributeData, int position)
    {
        attributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));

        // [WHY] Avoid LINQ ElementAtOrDefault to prevent delegate allocations.
        var typeArguments = attributeData.AttributeClass?.TypeArguments;
        if (typeArguments is { Length: > 0 } args && position >= 0 && position < args.Length)
        {
            return args[position];
        }

        return null;
    }

    /// <summary>Returns the named attribute argument matching the specified parameter name.</summary>
    public static TypedConstant GetNamedArgument(this AttributeData attributeData, string name)
    {
        attributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));

        // [WHY] Use foreach instead of LINQ FirstOrDefault(pair => ...) to eliminate predicate allocations.
        foreach (var pair in attributeData.NamedArguments)
        {
            if (pair.Key == name)
            {
                return pair.Value;
            }
        }

        return default;
    }
}
