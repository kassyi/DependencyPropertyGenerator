//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.NotNullStringProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="NotNullStringProperty"/> dependency property.<br/>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.UI.Xaml.DependencyProperty NotNullStringPropertyProperty =
global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "NotNullStringProperty",
propertyType: typeof(string),
ownerType: typeof(MyControl),
typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(
    defaultValue: (string)"",
    propertyChangedCallback: static (sender, args) =>
{
var coercedValue = ((MyControl)sender).CoerceNotNullStringProperty((string?)args.NewValue);
if (!global::System.Collections.Generic.EqualityComparer<string>.Default.Equals((string)args.NewValue, coercedValue))
{
((MyControl)sender).SetValue(NotNullStringPropertyProperty, coercedValue);
return;
}
var callback = new global::Microsoft.UI.Xaml.PropertyChangedCallback(static (sender, args) =>
{
((MyControl)sender).OnNotNullStringPropertyChanged();
((MyControl)sender).OnNotNullStringPropertyChanged((string)args.NewValue);
((MyControl)sender).OnNotNullStringPropertyChanged((string)args.OldValue, (string)args.NewValue);
});
callback(sender, args);
}
));

/// <summary>
/// Default value: ""
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string NotNullStringProperty
{
get => (string)GetValue(NotNullStringPropertyProperty);
set
{
if (value is null || value.Length == 0)
{
SetValue(NotNullStringPropertyProperty, value);
}
else
{
global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetPropertyFromString(this, NotNullStringPropertyProperty, value);
}
}

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnNotNullStringPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnNotNullStringPropertyChanged(string newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnNotNullStringPropertyChanged(string oldValue, string newValue);
private partial string CoerceNotNullStringProperty(string? value);
private static partial bool IsNotNullStringPropertyValid(string? value);
}
}
