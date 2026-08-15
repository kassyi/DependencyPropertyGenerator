//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyGrid
    {
        static MyGrid()
        {
            SomePropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string?>>(static x =>
            {
                ((MyGrid)x.Sender).OnSomePropertyChanged();
                ((MyGrid)x.Sender).OnSomePropertyChanged((string? )x.NewValue.GetValueOrDefault());
                ((MyGrid)x.Sender).OnSomePropertyChanged((string? )x.OldValue.GetValueOrDefault(), (string? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}