using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides string manipulation and normalization utilities for source generation.</summary>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Library extension methods for Source Generators")]
[SuppressMessage("ReSharper", "UnusedMethod.Global", Justification = "Library extension methods for Source Generators")]
[SuppressMessage("Roslynator", "RCS1163:Unused parameter", Justification = "Library extension methods for Source Generators")]
public static class StringExtensions
{
    /// <summary>Converts the input string to PascalCase suitable for a C# property name.</summary>
    public static string ToPropertyName(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
#if NET6_0_OR_GREATER
            _ => string.Concat(input[0].ToString().ToUpper(CultureInfo.InvariantCulture), input.AsSpan(1)),
#else
            _ => input[0].ToString().ToUpper(CultureInfo.InvariantCulture) + input[1..],
#endif
        };
    }

    /// <summary>Converts the input string to camelCase and escapes C# language keywords.</summary>
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "C# identifier and camelCase parameter normalization requires lowercase string comparison.")]
    public static string ToParameterName(this string input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
#pragma warning disable CA1308 // [WHY] Lowercase conversion (ToLowerInvariant) is essential for escaping C# reserved keywords.
        return input.ToLowerInvariant() switch
#pragma warning restore CA1308
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            
            // [WHY] Reference for C# keywords that require '@' prefix: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
            "abstract" => "@abstract",
            "as" => "@as",
            "base" => "@base",
            "bool" => "@bool",
            "break" => "@break",
            "byte" => "@byte",
            "case" => "@case",
            "catch" => "@catch",
            "cChar" => "@char",
            "checked" => "@checked",
            "class" => "@class",
            "const" => "@const",
            "continue" => "@continue",
            "decimal" => "@decimal",
            "default" => "@default",
            "delegate" => "@delegate",
            "do" => "@do",
            "double" => "@double",
            "else" => "@else",
            "enum" => "@enum",
            "event" => "@event",
            "explicit" => "@explicit",
            "extern" => "@extern",
            "false" => "@false",
            "finally" => "@finally",
            "fixed" => "@fixed",
            "float" => "@float",
            "for" => "@for",
            "foreach" => "@foreach",
            "goto" => "@goto",
            "if" => "@if",
            "implicit" => "@implicit",
            "in" => "@in",
            "int" => "@int",
            "interface" => "@interface",
            "internal" => "@internal",
            "is" => "@is",
            "lock" => "@lock",
            "long" => "@long",
            "namespace" => "@namespace",
            "new" => "@new",
            "null" => "@null",
            "object" => "@object",
            "operator" => "@operator",
            "out" => "@out",
            "override" => "@override",
            "params" => "@params",
            "private" => "@private",
            "protected" => "@protected",
            "public" => "@public",
            "readonly" => "@readonly",
            "ref" => "@ref",
            "return" => "@return",
            "sbyte" => "@sbyte",
            "sealed" => "@sealed",
            "short" => "@short",
            "sizeof" => "@sizeof",
            "stackalloc" => "@stackalloc",
            "static" => "@static",
            "string" => "@string",
            "struct" => "@struct",
            "switch" => "@switch",
            "this" => "@this",
            "throw" => "@throw",
            "true" => "@true",
            "try" => "@try",
            "typeof" => "@typeof",
            "uint" => "@uint",
            "ulong" => "@ulong",
            "unchecked" => "@unchecked",
            "unsafe" => "@unsafe",
            "ushort" => "@ushort",
            "using" => "@using",
            "virtual" => "@virtual",
            "void" => "@void",
            "volatile" => "@volatile",
            "while" => "@while",
#if NET6_0_OR_GREATER
            _ => string.Concat(input[0].ToString().ToLower(CultureInfo.InvariantCulture), input.AsSpan(1)),
#else
            _ => input[0].ToString().ToLower(CultureInfo.InvariantCulture) + input[1..],
#endif
#pragma warning restore CA1308 // Normalize strings to uppercase
        };
    }

    private static readonly char[] s_separator = ['\n'];

    /// <summary>Removes whitespace-only lines to maintain clean generated source layout.</summary>
    public static string RemoveBlankLinesWhereOnlyWhitespaces(this string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        return string.Join(
            separator: "\n",
            values: text
                .NormalizeLineEndings()
                .Split(s_separator, StringSplitOptions.None)
                .Where(static line => line.Length == 0 || !line.All(char.IsWhiteSpace)));
    }

    /// <summary>Normalizes line endings to the specified newline sequence (defaulting to '\n').</summary>
    public static string NormalizeLineEndings(
        this string text,
        string? newLine = null)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        // [WHY] Fast path: return original string immediately if text already uses Unix line endings and no custom newline is requested, avoiding string allocations.
        var targetNewLine = newLine ?? "\n";
        if (text.IndexOf('\r') < 0 && targetNewLine == "\n")
        {
            return text;
        }

        var newText = text
            .Replace("\r\n", "\n")
            .Replace("\r", "\n");
        if (newLine != null && newLine != "\n")
        {
            newText = newText.Replace("\n", newLine);
        }

        return newText;
    }

    /// <summary>Extracts the namespace portion from a fully qualified type name.</summary>
    public static string ExtractNamespace(this string fullTypeName)
    {
        fullTypeName = fullTypeName ?? throw new ArgumentNullException(nameof(fullTypeName));

        var lastDot = fullTypeName.LastIndexOf('.');
        return lastDot >= 0 ? fullTypeName[..lastDot] : string.Empty;
    }

    /// <summary>Extracts the unqualified type name from a fully qualified type name.</summary>
    public static string ExtractSimpleName(this string fullTypeName)
    {
        fullTypeName = fullTypeName ?? throw new ArgumentNullException(nameof(fullTypeName));

        var lastDot = fullTypeName.LastIndexOf('.');
        return lastDot >= 0 ? fullTypeName[(lastDot + 1)..] : fullTypeName;
    }

    /// <summary>Prepends the <c>global::</c> namespace alias to a fully qualified type name.</summary>
    public static string WithGlobalPrefix(this string fullTypeName)
    {
        fullTypeName = fullTypeName ?? throw new ArgumentNullException(nameof(fullTypeName));

        return $"global::{fullTypeName}";
    }

    private static readonly char[] s_specialChars = ['<', '>', ',', ' ', '_'];

    /// <summary>Sanitizes a type name or string injectively for use in Roslyn source generator hint names and file paths in a single pass without intermediate string allocations.</summary>
    public static string SanitizeFileName(this string input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));

        // Fast path: if there are no characters requiring escaping, return input with 0 allocations.
        if (input.IndexOfAny(s_specialChars) < 0)
        {
            return input;
        }

        var sb = new StringBuilder(input.Length + 16);
        foreach (var ch in input)
        {
            _ = ch switch
            {
                '<' => sb.Append("_lt_"),
                '>' => sb.Append("_gt_"),
                ',' => sb.Append("_comma_"),
                ' ' => sb.Append("_space_"),
                '_' => sb.Append("__"),
                _ => sb.Append(ch)
            };
        }

        return sb.ToString();
    }
}

