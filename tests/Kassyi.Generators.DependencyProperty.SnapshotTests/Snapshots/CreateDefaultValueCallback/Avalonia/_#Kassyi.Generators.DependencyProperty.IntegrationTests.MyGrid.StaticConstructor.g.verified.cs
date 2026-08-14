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
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)x.Sender).OnSomePropertyChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)x.Sender).OnSomePropertyChanged((string? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)x.Sender).OnSomePropertyChanged((string? )x.OldValue.GetValueOrDefault(), (string? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}