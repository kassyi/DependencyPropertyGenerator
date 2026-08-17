//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TreeViewExtensions.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class TreeViewExtensions
    {
        static TreeViewExtensions()
        {
            SelectedItemProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<object?>>(static x =>
            {
#pragma warning disable CS8600, CS8604
                OnSelectedItemChanged();
                OnSelectedItemChanged((global::Avalonia.Controls.TreeView)x.Sender);
                OnSelectedItemChanged((global::Avalonia.Controls.TreeView)x.Sender, (object? )x.NewValue.GetValueOrDefault());
                OnSelectedItemChanged((global::Avalonia.Controls.TreeView)x.Sender, (object? )x.OldValue.GetValueOrDefault(), (object? )x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
            }));
        }
    }
}