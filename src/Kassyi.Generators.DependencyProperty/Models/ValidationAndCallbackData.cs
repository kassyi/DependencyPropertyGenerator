using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct ValidationAndCallbackData(
    bool EnableDataValidation,
    bool Coerce,
    bool Validate,
    bool CreateDefaultValueCallback,
    EquatableArray<string> BindEvents,
    string OnChanged,
    EventCallbackData Callbacks);
