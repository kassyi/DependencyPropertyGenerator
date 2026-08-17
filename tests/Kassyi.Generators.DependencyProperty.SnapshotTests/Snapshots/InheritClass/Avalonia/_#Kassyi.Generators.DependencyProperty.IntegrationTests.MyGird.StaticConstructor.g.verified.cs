//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGird.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyGird
    {
        static MyGird()
        {
#pragma warning disable CS8600, CS8604
            MyColumnProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<int>>(static x =>
            {
                OnMyColumnChanged();
                OnMyColumnChanged((global::Avalonia.Controls.Control)x.Sender);
                OnMyColumnChanged((global::Avalonia.Controls.Control)x.Sender, (int)x.NewValue.GetValueOrDefault());
                OnMyColumnChanged((global::Avalonia.Controls.Control)x.Sender, (int)x.OldValue.GetValueOrDefault(), (int)x.NewValue.GetValueOrDefault());
            }));
#pragma warning restore CS8600, CS8604
        }
    }
}