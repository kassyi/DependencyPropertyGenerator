using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Diagnostics;

/// <summary>Centralized diagnostic descriptors and error definitions for DependencyPropertyGenerator.</summary>
internal static class DiagnosticDescriptors
{
    private const string UsageCategory = "Usage";

    /// <summary>DPG0000: The UI framework could not be recognized.</summary>
    public static readonly DiagnosticDescriptor FrameworkNotRecognized = new(
        id: "DPG0000",
        title: "Framework is not recognized",
        messageFormat: FrameworkExtensions.FrameworkIsNotRecognized,
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>DPG0001: The specified OnChanged method was not found or has an unsupported signature.</summary>
    public static readonly DiagnosticDescriptor CustomOnChangedMethodNotFoundOrUnsupported = new(
        id: "DPG0001",
        title: "OnChanged Method Not Found or Unsupported",
        messageFormat: "The specified OnChanged method '{0}' was not found or has an unsupported signature on '{1}'",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>Formats the C# #error preprocessor directive string for DPG0001.</summary>
    public static string FormatDpg0001Error(string onChangedMethodName, string fullClassName) =>
        $"#error DPG0001: The specified OnChanged method '{onChangedMethodName}' was not found or has an unsupported signature on '{fullClassName}'.";

    /// <summary>DPG0002: File scoped types are not supported.</summary>
    public static readonly DiagnosticDescriptor FileScopedTypeNotSupported = new(
        id: "DPG0002",
        title: "Invalid Type Modifier",
        messageFormat: "File scoped types are not supported by Source Generators ('{0}')",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>DPG0003: Ref struct property types are not supported.</summary>
    public static readonly DiagnosticDescriptor RefStructPropertyTypeNotSupported = new(
        id: "DPG0003",
        title: "Invalid Property Type",
        messageFormat: "The property type '{0}' is a ref struct and cannot be used as a DependencyProperty",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>DPG0004: Reference type default value sharing across instances.</summary>
    public static readonly DiagnosticDescriptor ReferenceTypeDefaultValueSharing = new(
        id: "DPG0004",
        title: "Reference Type Default Value Sharing",
        messageFormat: "Default value '{0}' is a reference type and will be shared across all instances. Use CreateDefaultValueCallback = true instead.",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>DPG0005: OldAndNewValue signature is not supported for OverrideMetadata on platforms without old value support.</summary>
    public static readonly DiagnosticDescriptor OverrideMetadataOldAndNewValueNotSupported = new(
        id: "DPG0005",
        title: "Invalid Callback Signature",
        messageFormat: "The OldAndNewValue signature is not supported for OverrideMetadata in {0} because RegisterPropertyChangedCallback does not provide the old value",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>DPG0007: Callback method has unsupported signature.</summary>
    public static readonly DiagnosticDescriptor UnsupportedCallbackSignature = new(
        id: "DPG0007",
        title: "Unsupported Callback Signature",
        messageFormat: "Method '{0}' matches the naming convention but has an unsupported signature",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>DPG0008: DefaultValueExpression contains invalid syntax.</summary>
    public static readonly DiagnosticDescriptor InvalidDefaultValueExpression = new(
        id: "DPG0008",
        title: "Invalid Default Value Expression",
        messageFormat: "The DefaultValueExpression '{0}' contains invalid syntax and could not be parsed",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>DPG0009: Suppresses CS0436 warnings caused by duplicate internal attribute helper types across friend assemblies.</summary>
    public static readonly SuppressionDescriptor Cs0436DuplicateInternalAttributeHelper = new(
        id: "DPG0009",
        suppressedDiagnosticId: "CS0436",
        justification: "DependencyPropertyGenerator emits internal attribute helper types into each compilation; duplicate friend-assembly copies are expected.");

    /// <summary>Unique diagnostic identifier for unhandled generator exceptions.</summary>
    public const string UnhandledExceptionId = "DPG9999";

    /// <summary>DPG9999: An unhandled exception occurred during source generation.</summary>
    public static readonly DiagnosticDescriptor UnhandledException = new(
        id: UnhandledExceptionId,
        title: "Unhandled Generator Exception",
        messageFormat: "An unhandled exception occurred during source generation: {0}",
        category: UsageCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
