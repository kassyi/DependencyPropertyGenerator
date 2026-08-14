//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            ReadOnlyPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<bool>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnReadOnlyPropertyChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnReadOnlyPropertyChanged((bool)x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnReadOnlyPropertyChanged((bool)x.OldValue.GetValueOrDefault(), (bool)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}