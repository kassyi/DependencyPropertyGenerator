namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents boolean flags and modifiers for a generated dependency property.</summary>
public readonly record struct PropertyModifiersData(
    bool IsValueType = true,
    bool IsSpecialType = false,
    bool IsReadOnly = false,
    bool IsDirect = false,
    bool IsAttached = false,
    bool IsAddOwner = false,
    bool IsPartialProperty = false,
    bool HidesBaseProperty = false,
    bool IsRequired = false,
    bool IsInitOnly = false);
