//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.CardBackground.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class MyControl
{
/// <summary>
/// Identifies the <see cref="CardBackground"/> dependency property.<br/>
/// Default value: default(Uri)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Avalonia.StyledProperty<global::System.Uri?> CardBackgroundProperty =
global::Avalonia.AvaloniaProperty.Register<MyControl, global::System.Uri?>(name: "CardBackground",
defaultValue: default(global::System.Uri),
inherits: false,
defaultBindingMode: global::Avalonia.Data.BindingMode.OneWay,
validate: null,
coerce: null);

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
