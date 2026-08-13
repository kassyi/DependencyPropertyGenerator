using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents validation, coercion, and change callback settings for a property.</summary>
public readonly record struct ValidationAndCallbackData(
    bool EnableDataValidation,
    bool Coerce,
    bool Validate,
    bool CreateDefaultValueCallback,
    EquatableArray<string> BindEvents,
    string OnChanged,
    EventCallbackData Callbacks);
