namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents custom XML documentation overrides for generated members.</summary>
public readonly record struct XmlDocumentationData(
    string? XmlDocumentation,
    string? GetterXmlDocumentation,
    string? SetterXmlDocumentation);
