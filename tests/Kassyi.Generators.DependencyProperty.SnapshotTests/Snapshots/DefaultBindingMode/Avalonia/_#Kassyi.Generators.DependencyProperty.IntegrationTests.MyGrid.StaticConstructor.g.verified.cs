//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyGrid
    {
        static MyGrid()
        {
            IsSpinningProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<bool>>(static x =>
            {
                ((MyGrid)x.Sender).OnIsSpinningChanged();
                ((MyGrid)x.Sender).OnIsSpinningChanged((bool)x.NewValue.GetValueOrDefault());
                ((MyGrid)x.Sender).OnIsSpinningChanged((bool)x.OldValue.GetValueOrDefault(), (bool)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}