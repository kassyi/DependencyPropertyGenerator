using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions;

/// <summary>Represents a generation result bundled with associated compilation diagnostics.</summary>
public readonly record struct ResultWithDiagnostics<T>(
    T Result,
    EquatableArray<Diagnostic> Diagnostics
)
{
    /// <summary>Creates a new instance with an empty diagnostic list.</summary>
    public ResultWithDiagnostics(T result) : this(result, ImmutableArray<Diagnostic>.Empty.AsEquatableArray())
    {
    }
}

/// <summary>Provides extension methods to wrap results with diagnostics.</summary>
public static class ResultWithDiagnosticsExtensions
{
    /// <summary>Wraps a value with an empty diagnostic set.</summary>
    public static ResultWithDiagnostics<T> ToResultWithDiagnostics<T>(this T result) => new(result);

    /// <summary>Wraps a value with the specified diagnostics.</summary>
    public static ResultWithDiagnostics<T> ToResultWithDiagnostics<T>(this T result, ImmutableArray<Diagnostic> diagnostics) => new(result, diagnostics.AsEquatableArray());
}
