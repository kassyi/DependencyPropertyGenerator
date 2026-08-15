//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Aquarium
    {
        static Aquarium()
        {
            AffectsRender<Aquarium>(AquariumSizeProperty);
            AquariumSizeProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                ((Aquarium)x.Sender).OnAquariumSizeChanged();
                ((Aquarium)x.Sender).OnAquariumSizeChanged((int)x.NewValue.GetValueOrDefault());
                ((Aquarium)x.Sender).OnAquariumSizeChanged((int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
        }
    }
}