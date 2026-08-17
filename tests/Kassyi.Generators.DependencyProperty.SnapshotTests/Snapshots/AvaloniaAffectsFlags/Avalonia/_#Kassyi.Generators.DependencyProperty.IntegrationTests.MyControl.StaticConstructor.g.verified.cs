//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            AffectsRender<MyControl>(FillProperty);
            AffectsMeasure<MyControl>(FillProperty);
            AffectsArrange<MyControl>(FillProperty);
            FillProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::Avalonia.Media.IBrush?>>(static x =>
            {
#pragma warning disable CS8600, CS8604
                ((MyControl)x.Sender).OnFillChanged();
                ((MyControl)x.Sender).OnFillChanged((global::Avalonia.Media.IBrush? )x.NewValue.GetValueOrDefault());
                ((MyControl)x.Sender).OnFillChanged((global::Avalonia.Media.IBrush? )x.OldValue.GetValueOrDefault(), (global::Avalonia.Media.IBrush? )x.NewValue.GetValueOrDefault());
#pragma warning restore CS8600, CS8604
            }));
        }
    }
}