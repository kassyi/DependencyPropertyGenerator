//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            ValuesProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<double[]?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnValuesChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnValuesChanged((double[]? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnValuesChanged((double[]? )x.OldValue.GetValueOrDefault(), (double[]? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}