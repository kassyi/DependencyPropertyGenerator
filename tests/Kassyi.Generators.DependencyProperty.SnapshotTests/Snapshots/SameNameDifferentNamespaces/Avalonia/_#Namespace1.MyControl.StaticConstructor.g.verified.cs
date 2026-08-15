//HintName: Namespace1.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Namespace1
{
    partial class MyControl
    {
        static MyControl()
        {
            MyPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((MyControl)x.Sender).OnMyPropertyChanged();
                ((MyControl)x.Sender).OnMyPropertyChanged((int)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnMyPropertyChanged((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}