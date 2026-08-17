//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TreeViewExtensions.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class TreeViewExtensions
{
static TreeViewExtensions()
{
ModeProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode>>(static x =>
{
#pragma warning disable CS8600, CS8604
OnModeChanged();
OnModeChanged((global::Avalonia.Controls.TreeView)x.Sender);
OnModeChanged((global::Avalonia.Controls.TreeView)x.Sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)x.NewValue.GetValueOrDefault());
OnModeChanged((global::Avalonia.Controls.TreeView)x.Sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)x.OldValue.GetValueOrDefault(), (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
}));
}
}
}
