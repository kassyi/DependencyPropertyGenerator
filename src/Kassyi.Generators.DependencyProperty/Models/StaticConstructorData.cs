using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents aggregated properties requiring static constructor registration for a class.</summary>
public readonly record struct StaticConstructorData(
    ClassData Class,
    EquatableArray<DependencyPropertyData> Properties
);
