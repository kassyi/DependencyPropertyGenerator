//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class GridExtensions
    {
        static GridExtensions()
        {
            SomePropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<object?>>(static x =>
            {
                OnSomePropertyChanged();
                OnSomePropertyChanged((global::Avalonia.AvaloniaObject)x.Sender);
                OnSomePropertyChanged((global::Avalonia.AvaloniaObject)x.Sender, (object? )x.NewValue.GetValueOrDefault());
                OnSomePropertyChanged((global::Avalonia.AvaloniaObject)x.Sender, (object? )x.OldValue.GetValueOrDefault(), (object? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}