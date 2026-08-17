//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.UIElementExtensions.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class UIElementExtensions
{
static UIElementExtensions()
{
BindEventPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<object?>>(static x =>
{
#pragma warning disable CS8600, CS8604
OnBindEventPropertyChanged();
OnBindEventPropertyChanged((global::Avalonia.Input.InputElement)x.Sender);
OnBindEventPropertyChanged((global::Avalonia.Input.InputElement)x.Sender, (object?)x.NewValue.GetValueOrDefault());
OnBindEventPropertyChanged((global::Avalonia.Input.InputElement)x.Sender, (object?)x.OldValue.GetValueOrDefault(), (object?)x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
}));
}
}
}
