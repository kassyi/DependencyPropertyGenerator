//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControlHelper.AttachedProperties.AttachedNotNullStringProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class MyControlHelper
{
/// <summary>
/// Identifies the AttachedNotNullStringProperty dependency property.<br/>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.UI.Xaml.DependencyProperty AttachedNotNullStringPropertyProperty =global::Microsoft.UI.Xaml.DependencyProperty.RegisterAttached(name: "AttachedNotNullStringProperty",
propertyType: typeof(string),
ownerType: typeof(MyControlHelper),
new global::Microsoft.UI.Xaml.PropertyMetadata(
    defaultValue: (string)"",
    propertyChangedCallback: static (sender, args) =>
{
var coercedValue = CoerceAttachedNotNullStringProperty((global::Microsoft.UI.Xaml.Controls.UserControl)sender, (string?)args.NewValue);
if (!global::System.Collections.Generic.EqualityComparer<string>.Default.Equals((string)args.NewValue, coercedValue))
{
((global::Microsoft.UI.Xaml.Controls.UserControl)sender).SetValue(AttachedNotNullStringPropertyProperty, coercedValue);
return;
}
var callback = new global::Microsoft.UI.Xaml.PropertyChangedCallback(static (sender, args) =>
{
OnAttachedNotNullStringPropertyChanged();
OnAttachedNotNullStringPropertyChanged((global::Microsoft.UI.Xaml.Controls.UserControl)sender);
OnAttachedNotNullStringPropertyChanged((global::Microsoft.UI.Xaml.Controls.UserControl)sender, (string)args.NewValue);
OnAttachedNotNullStringPropertyChanged((global::Microsoft.UI.Xaml.Controls.UserControl)sender, (string)args.OldValue, (string)args.NewValue);
});
callback(sender, args);
}
));
/// <summary>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetAttachedNotNullStringProperty(global::Microsoft.UI.Xaml.Controls.UserControl element, string value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
if (value is null || value.Length == 0)
{
element.SetValue(AttachedNotNullStringPropertyProperty, value);
}
else
{
global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetPropertyFromString(element, AttachedNotNullStringPropertyProperty, value);
}
}
/// <summary>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static string GetAttachedNotNullStringProperty(global::Microsoft.UI.Xaml.Controls.UserControl element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (string)element.GetValue(AttachedNotNullStringPropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged(global::Microsoft.UI.Xaml.Controls.UserControl userControl);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged(global::Microsoft.UI.Xaml.Controls.UserControl userControl, string newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedNotNullStringPropertyChanged(global::Microsoft.UI.Xaml.Controls.UserControl userControl, string oldValue, string newValue);
private static partial string CoerceAttachedNotNullStringProperty(global::Microsoft.UI.Xaml.Controls.UserControl userControl, string? value);
private static partial bool IsAttachedNotNullStringPropertyValid(string? value);
}
}
