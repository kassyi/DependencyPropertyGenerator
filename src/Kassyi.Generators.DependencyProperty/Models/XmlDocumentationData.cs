namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct XmlDocumentationData(
    string? XmlDocumentation,
    string? GetterXmlDocumentation,
    string? SetterXmlDocumentation);
