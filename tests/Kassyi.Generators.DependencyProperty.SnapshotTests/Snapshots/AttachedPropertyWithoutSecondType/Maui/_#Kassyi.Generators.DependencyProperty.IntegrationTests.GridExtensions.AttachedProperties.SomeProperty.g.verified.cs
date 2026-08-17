//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.AttachedProperties.SomeProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class GridExtensions
{
/// <summary>
/// Identifies the SomeProperty dependency property.<br/>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.Maui.Controls.BindableProperty SomePropertyProperty =global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(propertyName: "SomeProperty",
returnType: typeof(object),
declaringType: typeof(GridExtensions),
defaultValue: default(object),
defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay,
validateValue: null,
propertyChanged: static (sender, oldValue, newValue) =>
{
OnSomePropertyChanged();
OnSomePropertyChanged((global::Microsoft.Maui.Controls.BindableObject)sender);
OnSomePropertyChanged((global::Microsoft.Maui.Controls.BindableObject)sender, (object?)newValue);
OnSomePropertyChanged((global::Microsoft.Maui.Controls.BindableObject)sender, (object?)oldValue, (object?)newValue);
},
propertyChanging: static (sender, oldValue, newValue) =>
{
OnSomePropertyChanging();
OnSomePropertyChanging((global::Microsoft.Maui.Controls.BindableObject)sender);
OnSomePropertyChanging((global::Microsoft.Maui.Controls.BindableObject)sender, (object?)newValue);
OnSomePropertyChanging((global::Microsoft.Maui.Controls.BindableObject)sender, (object?)oldValue, (object?)newValue);
},
coerceValue: null,
defaultValueCreator: null);
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetSomeProperty(global::Microsoft.Maui.Controls.BindableObject element, object? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(SomePropertyProperty, value);
}
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static object? GetSomeProperty(global::Microsoft.Maui.Controls.BindableObject element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (object?)element.GetValue(SomePropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged(global::Microsoft.Maui.Controls.BindableObject bindableObject);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged(global::Microsoft.Maui.Controls.BindableObject bindableObject, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged(global::Microsoft.Maui.Controls.BindableObject bindableObject, object? oldValue, object? newValue);

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanging();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanging(global::Microsoft.Maui.Controls.BindableObject bindableObject);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanging(global::Microsoft.Maui.Controls.BindableObject bindableObject, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanging(global::Microsoft.Maui.Controls.BindableObject bindableObject, object? oldValue, object? newValue);
}
}
