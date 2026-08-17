//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridHelpers.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class GridHelpers
    {
        static GridHelpers()
        {
#pragma warning disable CS8600, CS8604
            RowCountProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                OnRowCountChanged((global::Avalonia.Controls.Grid)x.Sender, (int)x.NewValue.GetValueOrDefault());
            }));
#pragma warning restore CS8600, CS8604
        }
    }
}