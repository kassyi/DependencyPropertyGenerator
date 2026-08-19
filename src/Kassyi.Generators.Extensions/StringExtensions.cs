using System.Buffers;
using System.Diagnostics.CodeAnalysis;
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
            _ when char.IsUpper(input[0]) => input,
#if NET6_0_OR_GREATER
            _ => string.Create(input.Length, input, static (span, str) =>
            {
                span[0] = char.ToUpperInvariant(str[0]);
                str.AsSpan(1).CopyTo(span[1..]);
            }),
#else
            _ => char.ToUpperInvariant(input[0]) + input[1..],
#endif
        };
    }

    /// <summary>Converts the input string to camelCase and escapes C# language keywords.</summary>
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "C# identifier and camelCase parameter normalization requires lowercase string comparison.")]
    public static string ToParameterName(this string input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        if (input.Length == 0)
        {
            throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
        }

        if (char.IsLower(input[0]))
        {
            return input.EscapeKeyword();
        }

#if NET6_0_OR_GREATER
        var camelCased = string.Create(input.Length, input, static (span, str) =>
        {
            span[0] = char.ToLowerInvariant(str[0]);
            str.AsSpan(1).CopyTo(span[1..]);
        });
#else
        var camelCased = char.ToLowerInvariant(input[0]) + input[1..];
#endif
        return camelCased.EscapeKeyword();
    }

    private static bool IsCSharpKeyword(string input)
    {
        return input switch
        {
            "abstract" or "as" or "base" or "bool" or "break" or "byte" or "case" or
            "catch" or "char" or "checked" or "class" or "const" or "continue" or
            "decimal" or "default" or "delegate" or "do" or "double" or "else" or
            "enum" or "event" or "explicit" or "extern" or "false" or "finally" or
            "fixed" or "float" or "for" or "foreach" or "goto" or "if" or "implicit" or
            "in" or "int" or "interface" or "internal" or "is" or "lock" or "long" or
            "namespace" or "new" or "null" or "object" or "operator" or "out" or
            "override" or "params" or "private" or "protected" or "public" or
            "readonly" or "ref" or "return" or "sbyte" or "sealed" or "short" or
            "sizeof" or "stackalloc" or "static" or "string" or "struct" or "switch" or
            "this" or "throw" or "true" or "try" or "typeof" or "uint" or "ulong" or
            "unchecked" or "unsafe" or "ushort" or "using" or "virtual" or "void" or
            "volatile" or "while" => true,
            _ => false
        };
    }

    /// <summary>Escapes C# reserved keywords by prepending '@'.</summary>
    public static string EscapeKeyword(this string input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        
        return IsCSharpKeyword(input) ? "@" + input : input;
    }

    /// <summary>Removes whitespace-only lines to maintain clean generated source layout.</summary>
    public static string RemoveBlankLinesWhereOnlyWhitespaces(this string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));
        if (text.Length == 0)
        {
            return text;
        }

        var builder = new StringBuilder(text.Length);
        var start = 0;
        var hasCr = false;

        while (start < text.Length)
        {
            var end = start;
            var isAllWhiteSpace = true;

            // [WHY] Inspect line characters and whitespace status in a single pass without pre-normalizing line endings.
            while (end < text.Length && text[end] != '\r' && text[end] != '\n')
            {
                if (isAllWhiteSpace && !char.IsWhiteSpace(text[end]))
                {
                    isAllWhiteSpace = false;
                }
                end++;
            }

            // Keep pure empty lines (start == end) or lines containing non-whitespace characters
            if (!isAllWhiteSpace || start == end)
            {
                builder.Append(text, start, end - start);
                if (end < text.Length)
                {
                    builder.Append('\n');
                }
            }

            // Skip past \r\n, \r, or \n
            if (end < text.Length && text[end] == '\r')
            {
                hasCr = true;
                if (end + 1 < text.Length && text[end + 1] == '\n')
                {
                    end++;
                }
            }

            start = end + 1;
        }

        return builder.Length == text.Length && !hasCr ? text : builder.ToString();
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

        return fullTypeName.StartsWith("global::", StringComparison.Ordinal) ? fullTypeName : $"global::{fullTypeName}";
    }

#if NET8_0_OR_GREATER
    private static readonly SearchValues<char> s_specialChars = SearchValues.Create("<>, _@");
#else
    private static readonly char[] s_specialChars = ['<', '>', ',', ' ', '_', '@'];
#endif

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
                '@' => sb.Append("_at_"),
                _ => sb.Append(ch)
            };
        }

        return sb.ToString();
    }

    /// <summary>Resolves the corresponding XamlBindingHelper.SetPropertyFrom* method name for a given type, or null if unsupported.</summary>
    public static string? GetXamlBindingHelperSetMethodName(this string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return null;
        }

        // Nullable value types are not supported by SetPropertyFrom*
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        if (type.EndsWith('?') &&
#else
        if (type![type.Length - 1] == '?' &&
#endif
            type is not ("string?" or "String?" or "System.String?" or "global::System.String?"))
        {
            return null;
        }

        return type.ExtractSimpleName() switch
        {
            "bool" or "Boolean" => "SetPropertyFromBoolean",
            "byte" or "Byte" => "SetPropertyFromByte",
            "char" or "Char" => "SetPropertyFromChar16",
            "double" or "Double" => "SetPropertyFromDouble",
            "int" or "Int32" => "SetPropertyFromInt32",
            "long" or "Int64" => "SetPropertyFromInt64",
            "float" or "Single" => "SetPropertyFromSingle",
            "string" or "String" or "string?" or "String?" => "SetPropertyFromString",
            "uint" or "UInt32" => "SetPropertyFromUInt32",
            "ulong" or "UInt64" => "SetPropertyFromUInt64",
            "TimeSpan" => "SetPropertyFromTimeSpan",
            "DateTimeOffset" => "SetPropertyFromDateTime",
            "Point" => "SetPropertyFromPoint",
            "Rect" => "SetPropertyFromRect",
            "Size" => "SetPropertyFromSize",
            _ => null
        };
    }
}

