//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        static MyControl()
        {
            TypeIntStringProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<(int, string)>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeIntStringChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeIntStringChanged(((int, string))x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeIntStringChanged(((int, string))x.OldValue.GetValueOrDefault(), ((int, string))x.NewValue.GetValueOrDefault());
            }));
            TypeControlIntProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<(global::Avalonia.Controls.Control, int)>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeControlIntChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeControlIntChanged(((global::Avalonia.Controls.Control, int))x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeControlIntChanged(((global::Avalonia.Controls.Control, int))x.OldValue.GetValueOrDefault(), ((global::Avalonia.Controls.Control, int))x.NewValue.GetValueOrDefault());
            }));
            TypeIntControlProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<(int, global::Avalonia.Controls.Control)>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeIntControlChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeIntControlChanged(((int, global::Avalonia.Controls.Control))x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTypeIntControlChanged(((int, global::Avalonia.Controls.Control))x.OldValue.GetValueOrDefault(), ((int, global::Avalonia.Controls.Control))x.NewValue.GetValueOrDefault());
            }));
            TupleIntStringProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::System.Tuple<int, string>?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleIntStringChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleIntStringChanged((global::System.Tuple<int, string>? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleIntStringChanged((global::System.Tuple<int, string>? )x.OldValue.GetValueOrDefault(), (global::System.Tuple<int, string>? )x.NewValue.GetValueOrDefault());
            }));
            TupleControlIntProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::System.Tuple<global::Avalonia.Controls.Control, int>?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleControlIntChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleControlIntChanged((global::System.Tuple<global::Avalonia.Controls.Control, int>? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleControlIntChanged((global::System.Tuple<global::Avalonia.Controls.Control, int>? )x.OldValue.GetValueOrDefault(), (global::System.Tuple<global::Avalonia.Controls.Control, int>? )x.NewValue.GetValueOrDefault());
            }));
            TupleIntControlProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<global::System.Tuple<int, global::Avalonia.Controls.Control>?>>(static x =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleIntControlChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleIntControlChanged((global::System.Tuple<int, global::Avalonia.Controls.Control>? )x.NewValue.GetValueOrDefault());
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)x.Sender).OnTupleIntControlChanged((global::System.Tuple<int, global::Avalonia.Controls.Control>? )x.OldValue.GetValueOrDefault(), (global::System.Tuple<int, global::Avalonia.Controls.Control>? )x.NewValue.GetValueOrDefault());
            }));
        }
    }
}