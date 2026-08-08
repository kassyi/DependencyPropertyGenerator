using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct ClassData(
    string Namespace,
    string Name,
    string FullName,
    string Type,
    string Modifiers,
    string Version,
    bool IsStatic,
    Framework Framework);
