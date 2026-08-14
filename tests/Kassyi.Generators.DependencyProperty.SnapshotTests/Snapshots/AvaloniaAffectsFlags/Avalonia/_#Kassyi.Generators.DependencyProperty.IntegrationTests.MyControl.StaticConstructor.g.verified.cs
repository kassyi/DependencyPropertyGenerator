//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            AffectsRender<global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl>(FillProperty);
            AffectsMeasure<global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl>(FillProperty);
            AffectsArrange<global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl>(FillProperty);
            FillProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::Avalonia.Media.IBrush?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnFillChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnFillChanged((global::Avalonia.Media.IBrush? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnFillChanged((global::Avalonia.Media.IBrush? )x.OldValue.GetValueOrDefault(), (global::Avalonia.Media.IBrush? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}