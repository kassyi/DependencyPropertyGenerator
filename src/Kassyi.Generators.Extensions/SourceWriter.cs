#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kassyi.Generators.Extensions;

/// <summary>
/// A zero-allocation (or minimal-allocation) writer for source generators.
/// Internally uses a thread-static StringBuilder to avoid allocations during source string building.
/// </summary>
public readonly struct SourceWriter : IDisposable, IEquatable<SourceWriter>
{
    [ThreadStatic]
    private static StringBuilder[]? _threadStaticBuilders;

    [ThreadStatic]
    private static int _depth;

    private readonly int _myDepth;

    private StringBuilder Builder { get; }

    public SourceWriter()
    {
        _threadStaticBuilders ??= new StringBuilder[8];
        _myDepth = _depth++;

        if (_myDepth < _threadStaticBuilders.Length)
        {
            Builder = _threadStaticBuilders[_myDepth] ??= new StringBuilder(4096);
        }
        else
        {
            Builder = new StringBuilder(4096);
        }
        Builder.Clear();
    }

    public void Dispose()
    {
        if (_myDepth == _depth - 1)
        {
            _depth--;
        }
    }

    public void Append(string? value)
    {
        if (value is not null)
        {
            Builder.Append(value);
        }
    }

    public void Append(char value)
    {
        Builder.Append(value);
    }

    public void Append([InterpolatedStringHandlerArgument("")] ref SourceWriterInterpolatedStringHandler handler)
    {
        // The handler appends directly to the builder.
    }

    public void AppendLine()
    {
        Builder.AppendLine();
    }

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

    public void AppendLine([InterpolatedStringHandlerArgument("")] ref SourceWriterInterpolatedStringHandler handler)
    {
        Builder.AppendLine();
    }

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
        // The handler appends directly to the builder if the condition is true.
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

    public override string ToString()
    {
        return Builder.ToString();
    }

    public int Length => Builder.Length;

    internal StringBuilder GetBuilder() => Builder;

    /// <summary>
    /// The handler that enables zero-allocation string interpolation using <see cref="SourceWriter"/>.
    /// </summary>
    [InterpolatedStringHandler]
    public ref struct SourceWriterInterpolatedStringHandler
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

        public void AppendLiteral(string s)
        {
            _builder?.Append(s);
        }

        public void AppendFormatted(string? s)
        {
            if (s is not null)
            {
                _builder?.Append(s);
            }
        }

        public void AppendFormatted<T>(T t)
        {
            if (t is not null)
            {
                _builder?.Append(t?.ToString());
            }
        }

        public void AppendFormatted<T>(T t, string format) where T : IFormattable
        {
            if (t is not null)
            {
                _builder?.Append(t.ToString(format, null));
            }
        }
    }

    public bool Equals(SourceWriter other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is SourceWriter other && Equals(other);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(SourceWriter left, SourceWriter right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SourceWriter left, SourceWriter right)
    {
        return !(left == right);
    }

}
