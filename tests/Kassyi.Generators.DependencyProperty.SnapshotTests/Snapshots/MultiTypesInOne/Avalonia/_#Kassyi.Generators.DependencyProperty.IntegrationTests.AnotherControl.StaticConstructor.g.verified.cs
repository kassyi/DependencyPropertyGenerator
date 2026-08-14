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
                ((AnotherControl)x.Sender).OnMyProperty3Changed();
                ((AnotherControl)x.Sender).OnMyProperty3Changed((int)x.NewValue.GetValueOrDefault());
                ((AnotherControl)x.Sender).OnMyProperty3Changed((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
            MyPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((AnotherControl)x.Sender).OnMyPropertyChanged();
                ((AnotherControl)x.Sender).OnMyPropertyChanged((int)x.NewValue.GetValueOrDefault());
                ((AnotherControl)x.Sender).OnMyPropertyChanged((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
            MyProperty2Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<(int, string)>>(static x =>
            {
                ((AnotherControl)x.Sender).OnMyProperty2Changed();
                ((AnotherControl)x.Sender).OnMyProperty2Changed(((int, string))x.NewValue.GetValueOrDefault());
                ((AnotherControl)x.Sender).OnMyProperty2Changed(((int, string))x.OldValue.GetValueOrDefault(), ((int, string))x.NewValue.GetValueOrDefault());
            }));
        }
    }
}