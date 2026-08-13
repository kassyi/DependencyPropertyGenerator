namespace Kassyi.Generators.DependencyProperty.Models;

public readonly record struct EventCallbackData(
    CallbackSignature ChangedSignatures,
    CallbackSignature ChangingSignatures);
