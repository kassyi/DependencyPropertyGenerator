using System;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules;

internal static class SignatureRuleHelper
{
    internal static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    public static string GetNormalizedTypeName(ITypeSymbol typeSymbol)
    {
        var str = typeSymbol.ToDisplayString(TypeFormat);
        return str.EndsWith("?", StringComparison.Ordinal) ? str.Substring(0, str.Length - 1) : str;
    }

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

    public static bool IsEventArgsType(ITypeSymbol typeSymbol)
    {
        var current = typeSymbol;
        while (current != null)
        {
            if (current.Name is "DependencyPropertyChangedEventArgs" or "ValueChangedEventArgs")
            {
                return true;
            }

            if (current.Name == nameof(EventArgs) && current.ContainingNamespace?.ToDisplayString() == "System")
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }
}
