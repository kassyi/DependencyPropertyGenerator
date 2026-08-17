//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyUIElement.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class MyUIElement
{
static MyUIElement()
{
BindEventsPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<object?>>(static x =>
{
#pragma warning disable CS8600, CS8604
((MyUIElement)x.Sender).OnBindEventsPropertyChanged();
((MyUIElement)x.Sender).OnBindEventsPropertyChanged((object?)x.NewValue.GetValueOrDefault());
((MyUIElement)x.Sender).OnBindEventsPropertyChanged((object?)x.OldValue.GetValueOrDefault(), (object?)x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
}));
}
}
}
