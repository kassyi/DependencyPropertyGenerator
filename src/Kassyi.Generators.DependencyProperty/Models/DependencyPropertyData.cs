using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents all configuration and metadata for a generated dependency property.</summary>
public readonly record struct DependencyPropertyData(
    string Name,
    string Version,
    string Type,
    string ShortType,
    string? DefaultValue,
    string? DefaultValueDocumentation,
    Framework Framework,
    ComponentModelData ComponentModel,
    FrameworkMetadataData FrameworkMetadata,
    XmlDocumentationData XmlDocumentation,
    ValidationAndCallbackData ValidationAndCallbacks,
    PropertyModifiersData Modifiers);
