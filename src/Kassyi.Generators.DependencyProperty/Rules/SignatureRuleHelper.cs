using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Rules;

internal static class SignatureRuleHelper
{
    private static readonly SymbolDisplayFormat s_typeFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    public static string GetNormalizedTypeName(ITypeSymbol typeSymbol)
    {
        var str = typeSymbol.ToDisplayString(s_typeFormat);
        return str.EndsWith("?", StringComparison.Ordinal) ? str.Substring(0, str.Length - 1) : str;
    }

    public static bool IsEventArgsType(string typeName) =>
        typeName.EndsWith("EventArgs", StringComparison.Ordinal) ||
        typeName.EndsWith("EventArgs>", StringComparison.Ordinal) ||
        typeName.EndsWith("DependencyPropertyChangedEventArgs", StringComparison.Ordinal) ||
        typeName.EndsWith("ValueChangedEventArgs", StringComparison.Ordinal);
}
