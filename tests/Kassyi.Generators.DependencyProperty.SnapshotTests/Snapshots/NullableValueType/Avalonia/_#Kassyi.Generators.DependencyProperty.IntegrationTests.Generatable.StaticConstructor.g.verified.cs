//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Generatable
    {
        static Generatable()
        {
            PropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int?>>(static x =>
            {
                ((Generatable)x.Sender).OnPropertyChanged();
                ((Generatable)x.Sender).OnPropertyChanged((int? )x.NewValue.GetValueOrDefault());
                ((Generatable)x.Sender).OnPropertyChanged((int? )x.OldValue.GetValueOrDefault(), (int? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}