//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class AnotherControl
    {
        static AnotherControl()
        {
            MyProperty3Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)x.Sender).OnMyProperty3Changed();
            }));
            MyPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)x.Sender).OnMyPropertyChanged();
            }));
            MyProperty2Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<(int, string)>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)x.Sender).OnMyProperty2Changed();
            }));
        }
    }
}