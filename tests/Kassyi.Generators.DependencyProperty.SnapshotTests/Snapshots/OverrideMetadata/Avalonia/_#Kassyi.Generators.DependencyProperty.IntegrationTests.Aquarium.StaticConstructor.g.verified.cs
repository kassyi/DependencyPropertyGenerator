//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Aquarium
    {
        static Aquarium()
        {
            AffectsRender<global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium>(AquariumGraphicProperty);
            AquariumGraphicProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::System.Uri>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium)x.Sender).OnAquariumGraphicChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium)x.Sender).OnAquariumGraphicChanged((global::System.Uri)x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium)x.Sender).OnAquariumGraphicChanged((global::System.Uri)x.OldValue.GetValueOrDefault(), (global::System.Uri)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}