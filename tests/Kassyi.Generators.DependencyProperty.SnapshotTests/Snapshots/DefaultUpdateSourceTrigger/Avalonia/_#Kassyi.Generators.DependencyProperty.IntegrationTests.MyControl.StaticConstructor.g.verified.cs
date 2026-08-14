//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            ExplicitUpdateSourceTriggerPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<bool>>(static x =>
            {
                ((MyControl)x.Sender).OnExplicitUpdateSourceTriggerPropertyChanged();
                ((MyControl)x.Sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)x.OldValue.GetValueOrDefault(), (bool)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}