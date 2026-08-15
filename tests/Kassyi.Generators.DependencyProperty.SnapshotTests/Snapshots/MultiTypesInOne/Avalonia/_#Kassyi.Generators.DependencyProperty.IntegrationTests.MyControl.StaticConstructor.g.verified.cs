//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            MyProperty3Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((MyControl)x.Sender).OnMyProperty3Changed();
                ((MyControl)x.Sender).OnMyProperty3Changed((int)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnMyProperty3Changed((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
            MyPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((MyControl)x.Sender).OnMyPropertyChanged();
                ((MyControl)x.Sender).OnMyPropertyChanged((int)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnMyPropertyChanged((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
            MyProperty2Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((MyControl)x.Sender).OnMyProperty2Changed();
                ((MyControl)x.Sender).OnMyProperty2Changed((int)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnMyProperty2Changed((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}