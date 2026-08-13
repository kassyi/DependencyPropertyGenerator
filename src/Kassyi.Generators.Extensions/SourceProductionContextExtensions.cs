using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides diagnostic reporting extension methods for <see cref="SourceProductionContext"/>.</summary>
public static class SourceProductionContextExtensions
{
    /// <summary>Reports an unhandled exception as a compilation error diagnostic.</summary>
    public static void ReportException(
        this SourceProductionContext context,
        string id,
        Exception exception,
        string? prefix = null)
    {
        id = id ?? throw new ArgumentNullException(nameof(id));
        exception = exception ?? throw new ArgumentNullException(nameof(exception));

        context.ReportDiagnostic(exception.ToDiagnostic(id, prefix));
    }

    /// <summary>Creates a compiler error diagnostic representing an unhandled exception.</summary>
    public static Diagnostic ToDiagnostic(
        this Exception exception,
        string id,
        string? prefix = null)
    {
        exception = exception ?? throw new ArgumentNullException(nameof(exception));
        id = id ?? throw new ArgumentNullException(nameof(id));

        if (prefix != null)
        {
            id = $"{prefix}{id}";
        }

        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id,
                "Exception: ",
                $"{exception}",
                "Usage",
                DiagnosticSeverity.Error,
                true),
            Location.None);
    }
}
