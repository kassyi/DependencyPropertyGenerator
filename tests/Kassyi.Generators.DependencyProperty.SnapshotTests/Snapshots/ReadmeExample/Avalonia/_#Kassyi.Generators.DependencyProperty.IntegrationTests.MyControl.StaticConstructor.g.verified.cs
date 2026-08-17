//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class MyControl
{
static MyControl()
{
IsSpinningProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<bool>>(static x =>
{
#pragma warning disable CS8600, CS8604
((MyControl)x.Sender).OnIsSpinningChanged();
((MyControl)x.Sender).OnIsSpinningChanged((bool)x.NewValue.GetValueOrDefault());
((MyControl)x.Sender).OnIsSpinningChanged((bool)x.OldValue.GetValueOrDefault(), (bool)x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
}));
}
}
}
