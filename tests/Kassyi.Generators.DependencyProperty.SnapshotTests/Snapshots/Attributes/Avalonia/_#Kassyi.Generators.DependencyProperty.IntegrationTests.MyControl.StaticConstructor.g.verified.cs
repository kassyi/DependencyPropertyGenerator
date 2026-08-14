//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            AttributedPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnAttributedPropertyChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnAttributedPropertyChanged((string? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnAttributedPropertyChanged((string? )x.OldValue.GetValueOrDefault(), (string? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}