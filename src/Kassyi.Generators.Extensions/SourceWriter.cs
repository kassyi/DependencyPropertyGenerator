#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE0051 // Remove unused private members

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kassyi.Generators.Extensions;

/// <summary>A zero-allocation writer that builds source text using a pooled thread-static <see cref="StringBuilder"/>.</summary>
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Library methods for Source Generators")]
[SuppressMessage("ReSharper", "UnusedMethod.Global", Justification = "Library methods for Source Generators")]
[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Library methods for Source Generators")]
public readonly struct SourceWriter : IDisposable, IEquatable<SourceWriter>
{
    [ThreadStatic]
    private static StringBuilder?[]? s_threadStaticBuilders;

    [ThreadStatic]
    private static int s_depth;

    private readonly int _myDepth;

    private StringBuilder Builder { get; }

    public SourceWriter()
    {
        // [WHY] Reuses a thread-static StringBuilder pool to eliminate heap allocations during source generation.
        s_threadStaticBuilders ??= new StringBuilder[8];
        _myDepth = s_depth++;

        if (_myDepth < s_threadStaticBuilders.Length)
        {
            Builder = s_threadStaticBuilders[_myDepth] ??= new StringBuilder(4096);
        }
        else
        {
            Builder = new StringBuilder(4096);
        }
        Builder.Clear();
    }

    public void Dispose()
    {
        if (_myDepth == s_depth - 1)
        {
            s_depth--;
        }
    }

    public void Append(string? value)
    {
        if (value is not null)
        {
            Builder.Append(value);
        }
    }

    public void Append(char value) => Builder.Append(value);

#pragma warning disable CA1822 // Mark members as static
    public void Append([InterpolatedStringHandlerArgument("")] ref SourceWriterInterpolatedStringHandler handler)
    {
    }

    public void AppendLine() => Builder.AppendLine();

    public void AppendLine(string? value)
    {
        if (value is not null)
        {
            Builder.AppendLine(value);
        }
        else
        {
            Builder.AppendLine();
        }
    }

    public void AppendLine([InterpolatedStringHandlerArgument("")] ref SourceWriterInterpolatedStringHandler handler) => Builder.AppendLine();

    public void Line() => AppendLine();
    
    public void Line(string? value) => AppendLine(value);

    public void Line([InterpolatedStringHandlerArgument("")] ref SourceWriterInterpolatedStringHandler handler) => AppendLine(ref handler);

    public void AppendIf(bool condition, string? value)
    {
        if (condition && value is not null)
        {
            Builder.Append(value);
        }
    }

    public void AppendIf(bool condition, [InterpolatedStringHandlerArgument("", "condition")] ref SourceWriterInterpolatedStringHandler handler)
    {
    }

    public void AppendLineIf(bool condition, string? value)
    {
        if (condition && value is not null)
        {
            Builder.AppendLine(value);
        }
    }

    public void AppendLineIf(bool condition, [InterpolatedStringHandlerArgument("", "condition")] ref SourceWriterInterpolatedStringHandler handler)
    {
        if (condition)
        {
            Builder.AppendLine();
        }
    }

    public void LineIf(bool condition, string? value) => AppendLineIf(condition, value);

    public void LineIf(bool condition, [InterpolatedStringHandlerArgument("", "condition")] ref SourceWriterInterpolatedStringHandler handler) => AppendLineIf(condition, ref handler);

    public override string ToString() => Builder.ToString();

    public int Length => Builder.Length;

    internal StringBuilder GetBuilder() => Builder;

#pragma warning restore CA1822

    public bool Equals(SourceWriter other) => this == other;

    public override bool Equals(object? obj) => obj is SourceWriter other && Equals(other);

    public override int GetHashCode() => _myDepth;

    public SourceWriterScope Scope(string openingText = "{", string closingText = "}") => new(this, openingText, closingText);

    public static bool operator ==(SourceWriter left, SourceWriter right) => left.Equals(right);

    public static bool operator !=(SourceWriter left, SourceWriter right) => !(left == right);

}

/// <summary>A zero-allocation scope that appends an opening string on creation and a closing string on disposal.</summary>
public readonly ref struct SourceWriterScope
{
    private readonly SourceWriter _writer;
    private readonly string _closingText;

    public SourceWriterScope(SourceWriter writer, string openingText, string closingText)
    {
        _writer = writer;
        _closingText = closingText;
        if (string.IsNullOrEmpty(openingText))
        {
            return;
        }

        _writer.AppendLine(openingText);
#pragma warning disable CA1865
        if (openingText != "{" && !openingText.EndsWith("{", StringComparison.Ordinal))
#pragma warning restore CA1865
        {
            _writer.AppendLine("{");
        }
    }

    public void Dispose()
    {
        if (!string.IsNullOrEmpty(_closingText))
        {
            _writer.AppendLine(_closingText);
        }
    }
}

/// <summary>The handler that enables zero-allocation string interpolation using <see cref="SourceWriter"/>.</summary>
[InterpolatedStringHandler]
public readonly ref struct SourceWriterInterpolatedStringHandler
{
    private readonly StringBuilder? _builder;

    public SourceWriterInterpolatedStringHandler(int literalLength, int formattedCount, SourceWriter writer)
    {
        _builder = writer.GetBuilder();
    }

    public SourceWriterInterpolatedStringHandler(int literalLength, int formattedCount, SourceWriter writer, bool condition, out bool isHandlerEnabled)
    {
        if (condition)
        {
            _builder = writer.GetBuilder();
            isHandlerEnabled = true;
        }
        else
        {
            _builder = null;
            isHandlerEnabled = false;
        }
    }

    public void AppendLiteral(string s) => _builder?.Append(s);

    public void AppendFormatted(string? s)
    {
        if (s is not null)
        {
            _builder?.Append(s);
        }
    }

    public void AppendFormatted<T>(T t) => _builder?.Append(t);

    public void AppendFormatted<T>(T? t, string format) where T : IFormattable => _builder?.Append(t?.ToString(format, null));
}
