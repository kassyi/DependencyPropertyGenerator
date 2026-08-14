//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Generatable
    {
        static Generatable()
        {
            HeadersProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::System.Collections.Generic.Dictionary<string, string>?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable)x.Sender).OnHeadersChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable)x.Sender).OnHeadersChanged((global::System.Collections.Generic.Dictionary<string, string>? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable)x.Sender).OnHeadersChanged((global::System.Collections.Generic.Dictionary<string, string>? )x.OldValue.GetValueOrDefault(), (global::System.Collections.Generic.Dictionary<string, string>? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}