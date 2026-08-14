//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Generatable
    {
        static Generatable()
        {
            TextProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable)x.Sender).OnMyTextChanged((string? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}