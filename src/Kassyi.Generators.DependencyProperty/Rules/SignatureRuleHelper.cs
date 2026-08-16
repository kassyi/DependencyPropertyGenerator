using System;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules;

internal static class SignatureRuleHelper
{
    public static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    public static string GetNormalizedTypeName(ITypeSymbol typeSymbol)
    {
        var str = typeSymbol.ToDisplayString(TypeFormat);
        return str.EndsWith("?", StringComparison.Ordinal) ? str.Substring(0, str.Length - 1) : str;
    }

    public static bool IsEventArgsType(ITypeSymbol typeSymbol)
    {
        // Recursively check base types to see if it inherits from EventArgs
        // or has EventArgs in its name
        var current = typeSymbol;
        while (current != null)
        {
            if (current.Name is nameof(EventArgs) or "DependencyPropertyChangedEventArgs" or "ValueChangedEventArgs" ||
                current.Name.EndsWith(nameof(EventArgs), StringComparison.Ordinal))
            {
                return true;
            }
            current = current.BaseType;
        }

        return false;
    }
}
