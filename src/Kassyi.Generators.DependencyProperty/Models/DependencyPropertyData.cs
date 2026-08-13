using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct DependencyPropertyData(
    string Name,
    string Version,
    string Type,
    string ShortType,
    bool IsValueType,
    bool IsSpecialType,
    string? DefaultValue,
    string? DefaultValueDocumentation,
    bool IsReadOnly,
    bool IsDirect,
    bool IsAttached,
    bool IsAddOwner,
    Framework Framework,
    ComponentModelData ComponentModel,
    FrameworkMetadataData FrameworkMetadata,
    XmlDocumentationData XmlDocumentation,
    ValidationAndCallbackData ValidationAndCallbacks);
