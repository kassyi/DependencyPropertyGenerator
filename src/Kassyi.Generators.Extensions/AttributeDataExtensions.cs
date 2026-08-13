using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides extension methods for querying <see cref="AttributeData"/> arguments.</summary>
public static class AttributeDataExtensions
{
    /// <summary>Returns the generic type argument at the specified index, or <see langword="null"/> if not found.</summary>
    public static ITypeSymbol? GetGenericTypeArgument(this AttributeData attributeData, int position)
    {
        attributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));

        return attributeData.AttributeClass?.TypeArguments.ElementAtOrDefault(position);
    }

    /// <summary>Returns the named attribute argument matching the specified parameter name.</summary>
    public static TypedConstant GetNamedArgument(this AttributeData attributeData, string name)
    {
        attributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));

        return attributeData.NamedArguments
            .FirstOrDefault(pair => pair.Key == name)
            .Value;
    }
}
