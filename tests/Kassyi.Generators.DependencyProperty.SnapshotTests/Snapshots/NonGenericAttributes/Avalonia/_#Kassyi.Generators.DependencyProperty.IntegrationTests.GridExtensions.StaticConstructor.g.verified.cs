//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class GridExtensions
    {
        static GridExtensions()
        {
            AttachedPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<object?>>(static x =>
            {
                OnAttachedPropertyChanged();
                OnAttachedPropertyChanged((global::Avalonia.Controls.Grid)x.Sender);
                OnAttachedPropertyChanged((global::Avalonia.Controls.Grid)x.Sender, (object? )x.NewValue.GetValueOrDefault());
                OnAttachedPropertyChanged((global::Avalonia.Controls.Grid)x.Sender, (object? )x.OldValue.GetValueOrDefault(), (object? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}