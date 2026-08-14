using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents metadata about the target class declaring generated members.</summary>
public readonly record struct ClassData(
    string Namespace,
    string Name,
    string FullName,
    string Type,
    string Keyword,
    string NameWithTypeParameters,
    string Modifiers,
    string Version,
    bool IsStatic,
    Framework Framework,
    EquatableArray<ParentClassData> ParentClasses);
