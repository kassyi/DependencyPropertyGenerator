//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            NotNullStringPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string>>(static x =>
            {
                ((MyControl)x.Sender).OnNotNullStringPropertyChanged();
                ((MyControl)x.Sender).OnNotNullStringPropertyChanged((string)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnNotNullStringPropertyChanged((string)x.OldValue.GetValueOrDefault(), (string)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}