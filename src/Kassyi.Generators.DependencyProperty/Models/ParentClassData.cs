namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents metadata about an outer/parent class containing the target generated class.</summary>
public readonly record struct ParentClassData(
    string Keyword,
    string NameWithTypeParameters,
    string Modifiers);
