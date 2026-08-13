using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct StaticConstructorData(
    ClassData Class,
    EquatableArray<DependencyPropertyData> Properties
);
