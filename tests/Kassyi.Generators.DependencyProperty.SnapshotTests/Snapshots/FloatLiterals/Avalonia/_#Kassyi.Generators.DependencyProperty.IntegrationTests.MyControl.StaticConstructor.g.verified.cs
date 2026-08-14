//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            FloatPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<float>>(static x =>
            {
                ((MyControl)x.Sender).OnFloatPropertyChanged();
                ((MyControl)x.Sender).OnFloatPropertyChanged((float)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnFloatPropertyChanged((float)x.OldValue.GetValueOrDefault(), (float)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}