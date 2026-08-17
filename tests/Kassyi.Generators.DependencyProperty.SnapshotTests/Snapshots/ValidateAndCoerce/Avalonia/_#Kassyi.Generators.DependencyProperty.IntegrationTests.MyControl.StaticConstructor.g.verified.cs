//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
#pragma warning disable CS8600, CS8604
            NotNullStringPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string>>(static x =>
            {
                ((MyControl)x.Sender).OnNotNullStringPropertyChanged();
                ((MyControl)x.Sender).OnNotNullStringPropertyChanged((string)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnNotNullStringPropertyChanged((string)x.OldValue.GetValueOrDefault(), (string)x.NewValue.GetValueOrDefault());
            }));
#pragma warning restore CS8600, CS8604
        }
    }
}