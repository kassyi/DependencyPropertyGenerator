//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.CardBackground.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="CardBackground"/> dependency property.<br/>
/// Default value: default(Uri)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.UI.Xaml.DependencyProperty CardBackgroundProperty =
global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "CardBackground",
propertyType: typeof(global::System.Uri),
ownerType: typeof(MyControl),
typeMetadata: global::Microsoft.UI.Xaml.PropertyMetadata.Create(
    createDefaultValueCallback: static () => GetCardBackgroundDefaultValue(),
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnCardBackgroundChanged();
((MyControl)sender).OnCardBackgroundChanged((global::System.Uri?)args.NewValue);
((MyControl)sender).OnCardBackgroundChanged((global::System.Uri?)args.OldValue, (global::System.Uri?)args.NewValue);
}));

/// <summary>
/// Default value: default(Uri)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public global::System.Uri? CardBackground
{
get => (global::System.Uri?)GetValue(CardBackgroundProperty);
set => SetValue(CardBackgroundProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnCardBackgroundChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnCardBackgroundChanged(global::System.Uri? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnCardBackgroundChanged(global::System.Uri? oldValue, global::System.Uri? newValue);
private static partial global::System.Uri GetCardBackgroundDefaultValue();
}
}
