//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class MyControl
{
static MyControl()
{
CardBackgroundProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::System.Uri?>>(static x =>
{
#pragma warning disable CS8600, CS8604
((MyControl)x.Sender).OnCardBackgroundChanged();
((MyControl)x.Sender).OnCardBackgroundChanged((global::System.Uri?)x.NewValue.GetValueOrDefault());
((MyControl)x.Sender).OnCardBackgroundChanged((global::System.Uri?)x.OldValue.GetValueOrDefault(), (global::System.Uri?)x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
}));
}
}
}
