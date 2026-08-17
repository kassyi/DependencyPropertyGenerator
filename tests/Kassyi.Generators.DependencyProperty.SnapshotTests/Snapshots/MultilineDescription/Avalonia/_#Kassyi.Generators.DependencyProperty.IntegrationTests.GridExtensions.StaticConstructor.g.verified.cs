//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class GridExtensions
    {
        static GridExtensions()
        {
#pragma warning disable CS8600, CS8604
            UserAgentSuffixProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string?>>(static x =>
            {
                OnUserAgentSuffixChanged();
                OnUserAgentSuffixChanged((global::Avalonia.Controls.Grid)x.Sender);
                OnUserAgentSuffixChanged((global::Avalonia.Controls.Grid)x.Sender, (string? )x.NewValue.GetValueOrDefault());
                OnUserAgentSuffixChanged((global::Avalonia.Controls.Grid)x.Sender, (string? )x.OldValue.GetValueOrDefault(), (string? )x.NewValue.GetValueOrDefault());
            }));
#pragma warning restore CS8600, CS8604
        }
    }
}