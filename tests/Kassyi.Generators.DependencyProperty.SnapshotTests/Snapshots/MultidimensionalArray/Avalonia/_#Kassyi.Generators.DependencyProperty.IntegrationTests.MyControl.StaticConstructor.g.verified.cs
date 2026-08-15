//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            Values3Property.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int[,, ]?>>(static x =>
            {
                ((MyControl)x.Sender).OnValues3Changed();
                ((MyControl)x.Sender).OnValues3Changed((int[,, ]? )x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnValues3Changed((int[,, ]? )x.OldValue.GetValueOrDefault(), (int[,, ]? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}