namespace Kassyi.Generators.DependencyProperty.Models;

[Flags]
public enum CallbackSignature
{
    None = 0,
    NoParameters = 1 << 0,
    NewValue = 1 << 1,
    OldAndNewValue = 1 << 2,
    SenderAndOldAndNewValue = 1 << 3,
    EventArgs = 1 << 4,
    SenderAndEventArgs = 1 << 5
}
