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
public static readonly global::Microsoft.Maui.Controls.BindableProperty CardBackgroundProperty =
global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "CardBackground",
returnType: typeof(global::System.Uri),
declaringType: typeof(MyControl),
defaultValue: default(global::System.Uri),
defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay,
validateValue: null,
propertyChanged: static (sender, oldValue, newValue) =>
{
((MyControl)sender).OnCardBackgroundChanged();
((MyControl)sender).OnCardBackgroundChanged((global::System.Uri?)newValue);
((MyControl)sender).OnCardBackgroundChanged((global::System.Uri?)oldValue, (global::System.Uri?)newValue);
},
propertyChanging: static (sender, oldValue, newValue) =>
{
((MyControl)sender).OnCardBackgroundChanging();
((MyControl)sender).OnCardBackgroundChanging((global::System.Uri?)newValue);
((MyControl)sender).OnCardBackgroundChanging((global::System.Uri?)oldValue, (global::System.Uri?)newValue);
},
coerceValue: null,
defaultValueCreator: static _ => GetCardBackgroundDefaultValue());

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

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnCardBackgroundChanging();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnCardBackgroundChanging(global::System.Uri? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnCardBackgroundChanging(global::System.Uri? oldValue, global::System.Uri? newValue);
private static partial global::System.Uri GetCardBackgroundDefaultValue();
}
}
