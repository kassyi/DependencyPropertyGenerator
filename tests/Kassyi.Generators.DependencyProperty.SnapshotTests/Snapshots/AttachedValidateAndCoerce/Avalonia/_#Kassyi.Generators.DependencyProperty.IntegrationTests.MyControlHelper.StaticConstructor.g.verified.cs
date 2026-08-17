//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControlHelper.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControlHelper
    {
        static MyControlHelper()
        {
#pragma warning disable CS8600, CS8604
            AttachedNotNullStringPropertyProperty.Changed.Subscribe(new global::Avalonia.Reactive.AnonymousObserver<global::Avalonia.AvaloniaPropertyChangedEventArgs<string>>(static x =>
            {
                OnAttachedNotNullStringPropertyChanged();
                OnAttachedNotNullStringPropertyChanged((global::Avalonia.Controls.UserControl)x.Sender);
                OnAttachedNotNullStringPropertyChanged((global::Avalonia.Controls.UserControl)x.Sender, (string)x.NewValue.GetValueOrDefault());
                OnAttachedNotNullStringPropertyChanged((global::Avalonia.Controls.UserControl)x.Sender, (string)x.OldValue.GetValueOrDefault(), (string)x.NewValue.GetValueOrDefault());
            }));
#pragma warning restore CS8600, CS8604
        }
    }
}