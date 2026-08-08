namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct EventData(
    string Name,
    string Strategy,
    string Type,
    bool IsValueType,
    bool IsAttached,
    string? Description,
    string? Category,
    string? XmlDocumentation,
    string? EventXmlDocumentation,
    bool WinRtEvents);
