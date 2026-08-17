//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControlHelper.AttachedProperties.AttachedNotNullStringProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class MyControlHelper
{
/// <summary>
/// Identifies the AttachedNotNullStringProperty dependency property.<br/>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Avalonia.AttachedProperty<string> AttachedNotNullStringPropertyProperty =global::Avalonia.AvaloniaProperty.RegisterAttached<MyControlHelper, global::Avalonia.Controls.UserControl, string>(name: "AttachedNotNullStringProperty",
defaultValue: (string)"",
inherits: false,
defaultBindingMode: global::Avalonia.Data.BindingMode.OneWay,
validate: static value => IsAttachedNotNullStringPropertyValid((string?)value),
coerce: static (sender, value) => CoerceAttachedNotNullStringProperty((global::Avalonia.Controls.UserControl)sender, (string?)value));
/// <summary>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetAttachedNotNullStringProperty(global::Avalonia.Controls.UserControl element, string value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(AttachedNotNullStringPropertyProperty, value);
}
/// <summary>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static string GetAttachedNotNullStringProperty(global::Avalonia.Controls.UserControl element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (string)element.GetValue(AttachedNotNullStringPropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged(global::Avalonia.Controls.UserControl userControl);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged(global::Avalonia.Controls.UserControl userControl, string newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged(global::Avalonia.Controls.UserControl userControl, string oldValue, string newValue);
private static partial string CoerceAttachedNotNullStringProperty(global::Avalonia.Controls.UserControl userControl, string? value);
private static partial bool IsAttachedNotNullStringPropertyValid(string? value);
}
}
