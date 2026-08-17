//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class MyControl
{
static MyControl()
{
AttributedPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string?>>(static x =>
{
#pragma warning disable CS8600, CS8604
((MyControl)x.Sender).OnAttributedPropertyChanged();
((MyControl)x.Sender).OnAttributedPropertyChanged((string?)x.NewValue.GetValueOrDefault());
((MyControl)x.Sender).OnAttributedPropertyChanged((string?)x.OldValue.GetValueOrDefault(), (string?)x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
}));
}
}
}
