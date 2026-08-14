//HintName: Namespace2.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Namespace2
{
    partial class MyControl
    {
        static MyControl()
        {
            MyPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((global::Namespace2.MyControl)x.Sender).OnMyPropertyChanged();
                ((global::Namespace2.MyControl)x.Sender).OnMyPropertyChanged((int)x.NewValue.GetValueOrDefault());
                ((global::Namespace2.MyControl)x.Sender).OnMyPropertyChanged((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}