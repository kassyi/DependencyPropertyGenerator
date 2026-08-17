using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules;

/// <summary>Provides utility methods for resolving and comparing type signatures in callback methods.</summary>
internal static class SignatureRuleHelper
{
    internal static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    /// <summary>Returns the fully qualified type name without the global namespace prefix and trailing question marks.</summary>
    public static string GetNormalizedTypeName(ITypeSymbol typeSymbol)
    {
        var str = typeSymbol.ToDisplayString(TypeFormat);
        return str.EndsWith("?", StringComparison.Ordinal) ? str.Substring(0, str.Length - 1) : str;
    }

    /// <summary>Trims global:: and trailing nullable markers from a type name string.</summary>
    internal static string NormalizeTypeName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return string.Empty;
        }

        var span = typeName.AsSpan();
        if (span.StartsWith("global::".AsSpan(), StringComparison.Ordinal))
        {
            span = span.Slice("global::".Length);
        }
        if (span.EndsWith("?".AsSpan(), StringComparison.Ordinal))
        {
            span = span.Slice(0, span.Length - 1);
        }

        return span.Length == typeName.Length ? typeName : span.ToString();
    }

    /// <summary>Determines if a given type symbol represents an EventArgs derivative used in event callbacks.</summary>
    public static bool IsEventArgsType(ITypeSymbol typeSymbol)
    {
        var current = typeSymbol;
        while (current != null)
        {
            switch (current.Name)
            {
                case "DependencyPropertyChangedEventArgs" or "ValueChangedEventArgs":
                case nameof(EventArgs) when (current.ContainingNamespace?.ToDisplayString() == "System"):
                    return true;
                default:
                    current = current.BaseType;
                    break;
            }
        }

        return false;
    }
}
