using System.Globalization;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.Extensions.Models;

/// <summary>
/// Exception that carries a Roslyn <see cref="Microsoft.CodeAnalysis.Diagnostic"/> to be reported by the generator.
/// </summary>
public sealed class DiagnosticException : Exception
{
    /// <summary>
    /// Gets the diagnostic associated with this exception.
    /// </summary>
    public Diagnostic? Diagnostic { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticException"/> class.
    /// </summary>
    public DiagnosticException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DiagnosticException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DiagnosticException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticException"/> class wrapping a <see cref="Microsoft.CodeAnalysis.Diagnostic"/>.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to report.</param>
    public DiagnosticException(Diagnostic diagnostic)
        : base((diagnostic ?? throw new ArgumentNullException(nameof(diagnostic))).GetMessage(CultureInfo.InvariantCulture))
    {
        Diagnostic = diagnostic;
    }
}
