//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class GridExtensions
    {
        static GridExtensions()
        {
#pragma warning disable CS8600, CS8604
            AttachedReadOnlyPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<object?>>(static x =>
            {
                OnAttachedReadOnlyPropertyChanged();
                OnAttachedReadOnlyPropertyChanged((global::Avalonia.Controls.Grid)x.Sender);
                OnAttachedReadOnlyPropertyChanged((global::Avalonia.Controls.Grid)x.Sender, (object? )x.NewValue.GetValueOrDefault());
                OnAttachedReadOnlyPropertyChanged((global::Avalonia.Controls.Grid)x.Sender, (object? )x.OldValue.GetValueOrDefault(), (object? )x.NewValue.GetValueOrDefault());
            }));
#pragma warning restore CS8600, CS8604
        }
    }
}