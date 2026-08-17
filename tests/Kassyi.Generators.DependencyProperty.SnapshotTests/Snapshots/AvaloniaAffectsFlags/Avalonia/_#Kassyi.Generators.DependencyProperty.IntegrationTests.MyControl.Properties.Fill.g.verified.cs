//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.Fill.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
partial class MyControl
{
/// <summary>
/// Identifies the <see cref="Fill"/> dependency property.<br/>
/// Default value: default(Brush)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Avalonia.StyledProperty<global::Avalonia.Media.Brush?> FillProperty =
global::Avalonia.AvaloniaProperty.Register<MyControl, global::Avalonia.Media.Brush?>(name: "Fill",
defaultValue: default(global::Avalonia.Media.Brush),
inherits: false,
defaultBindingMode: global::Avalonia.Data.BindingMode.OneWay,
validate: null,
coerce: null);

/// <summary>
/// Default value: default(Brush)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public global::Avalonia.Media.Brush? Fill
{
get => (global::Avalonia.Media.Brush?)GetValue(FillProperty);
set => SetValue(FillProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnFillChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnFillChanged(global::Avalonia.Media.Brush? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnFillChanged(global::Avalonia.Media.Brush? oldValue, global::Avalonia.Media.Brush? newValue);
}
}
