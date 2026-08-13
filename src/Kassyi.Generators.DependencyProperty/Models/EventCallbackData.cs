namespace Kassyi.Generators.DependencyProperty.Models;

/// <summary>Represents callback signature patterns matched on the declaring class.</summary>
public readonly record struct EventCallbackData(
    CallbackSignature ChangedSignatures,
    CallbackSignature ChangingSignatures);
