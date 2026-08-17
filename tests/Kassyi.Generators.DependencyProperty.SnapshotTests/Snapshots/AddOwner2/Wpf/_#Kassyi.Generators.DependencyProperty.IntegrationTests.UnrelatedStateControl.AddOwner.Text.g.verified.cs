//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.UnrelatedStateControl.AddOwner.Text.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class UnrelatedStateControl
{
/// <summary>
/// Identifies the <see cref="Text"/> dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty TextProperty =
global::System.Windows.Controls.TextBox.TextProperty.AddOwner(ownerType: typeof(UnrelatedStateControl), null);
/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string? Text
{
get => (string?)GetValue(TextProperty);
set => SetValue(TextProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnTextChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnTextChanged(string? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnTextChanged(string? oldValue, string? newValue);
}
}
