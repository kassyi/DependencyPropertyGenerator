//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            TextProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string?>>(static x =>
            {
                ((MyControl)x.Sender).OnTextChanged();
                ((MyControl)x.Sender).OnTextChanged((string? )x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnTextChanged((string? )x.OldValue.GetValueOrDefault(), (string? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}