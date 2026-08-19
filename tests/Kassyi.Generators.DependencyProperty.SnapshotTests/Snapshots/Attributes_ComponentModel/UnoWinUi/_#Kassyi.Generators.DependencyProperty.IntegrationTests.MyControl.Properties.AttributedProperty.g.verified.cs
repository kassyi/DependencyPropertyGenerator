//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.AttributedProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="AttributedProperty"/> dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.UI.Xaml.DependencyProperty AttributedPropertyProperty =
global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "AttributedProperty",
propertyType: typeof(string),
ownerType: typeof(MyControl),
typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(
    defaultValue: default(string),
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnAttributedPropertyChanged();
((MyControl)sender).OnAttributedPropertyChanged((string?)args.NewValue);
((MyControl)sender).OnAttributedPropertyChanged((string?)args.OldValue, (string?)args.NewValue);
}));

/// <summary>
/// Description<br/>
/// Default value: default(string)
/// </summary>
[global::System.ComponentModel.Category("Category")]
[global::System.ComponentModel.Description("Description")]
[global::System.ComponentModel.TypeConverter(typeof(global::System.ComponentModel.EnumConverter))]
[global::System.ComponentModel.Bindable(true)]
[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
[global::System.CLSCompliant(false)]
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string? AttributedProperty
{
get => (string?)GetValue(AttributedPropertyProperty);
set
{
if (value is null || value.Length == 0)
{
SetValue(AttributedPropertyProperty, value);
}
else
{
global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetPropertyFromString(this, AttributedPropertyProperty, value);
}
}

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAttributedPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAttributedPropertyChanged(string? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAttributedPropertyChanged(string? oldValue, string? newValue);
}
}
