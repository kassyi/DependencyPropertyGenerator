//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.UIElementExtensions.AttachedProperties.BindEventProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class UIElementExtensions
{
/// <summary>
/// Identifies the BindEventProperty dependency property.<br/>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.Maui.Controls.BindableProperty BindEventPropertyProperty =global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(propertyName: "BindEventProperty",
returnType: typeof(object),
declaringType: typeof(UIElementExtensions),
defaultValue: default(object),
defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay,
validateValue: null,
propertyChanged: static (sender, oldValue, newValue) =>
{
OnBindEventPropertyChanged();
OnBindEventPropertyChanged((global::Microsoft.Maui.Controls.VisualElement)sender);
OnBindEventPropertyChanged((global::Microsoft.Maui.Controls.VisualElement)sender, (object?)newValue);
OnBindEventPropertyChanged((global::Microsoft.Maui.Controls.VisualElement)sender, (object?)oldValue, (object?)newValue);
},
propertyChanging: static (sender, oldValue, newValue) =>
{
OnBindEventPropertyChanging();
OnBindEventPropertyChanging((global::Microsoft.Maui.Controls.VisualElement)sender);
OnBindEventPropertyChanging((global::Microsoft.Maui.Controls.VisualElement)sender, (object?)newValue);
OnBindEventPropertyChanging((global::Microsoft.Maui.Controls.VisualElement)sender, (object?)oldValue, (object?)newValue);
},
coerceValue: null,
defaultValueCreator: null);
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetBindEventProperty(global::Microsoft.Maui.Controls.VisualElement element, object? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(BindEventPropertyProperty, value);
}
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static object? GetBindEventProperty(global::Microsoft.Maui.Controls.VisualElement element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (object?)element.GetValue(BindEventPropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanged(global::Microsoft.Maui.Controls.VisualElement visualElement);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanged(global::Microsoft.Maui.Controls.VisualElement visualElement, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanged(global::Microsoft.Maui.Controls.VisualElement visualElement, object? oldValue, object? newValue);

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanging();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanging(global::Microsoft.Maui.Controls.VisualElement visualElement);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanging(global::Microsoft.Maui.Controls.VisualElement visualElement, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnBindEventPropertyChanging(global::Microsoft.Maui.Controls.VisualElement visualElement, object? oldValue, object? newValue);

static partial void OnBindEventPropertyChanged_BeforeBind(global::Microsoft.Maui.Controls.VisualElement visualElement, object? oldValue, object? newValue);
static partial void OnBindEventPropertyChanged_AfterBind(global::Microsoft.Maui.Controls.VisualElement visualElement, object? oldValue, object? newValue);

static partial void OnBindEventPropertyChanged(global::Microsoft.Maui.Controls.VisualElement visualElement, object? oldValue, object? newValue)
{
OnBindEventPropertyChanged_BeforeBind(visualElement, oldValue, newValue);

if (oldValue is not default(object))
{
visualElement.SizeChanged -= OnBindEventPropertyChanged_SizeChanged;
}
if (newValue is not default(object))
{
visualElement.SizeChanged += OnBindEventPropertyChanged_SizeChanged;
}

OnBindEventPropertyChanged_AfterBind(visualElement, oldValue, newValue);
}
}
}
