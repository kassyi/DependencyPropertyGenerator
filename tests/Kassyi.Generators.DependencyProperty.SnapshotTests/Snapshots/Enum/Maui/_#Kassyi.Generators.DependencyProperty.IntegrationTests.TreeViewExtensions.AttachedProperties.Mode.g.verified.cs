//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TreeViewExtensions.AttachedProperties.Mode.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class TreeViewExtensions
{
/// <summary>
/// Identifies the Mode dependency property.<br/>
/// Default value: Mode2
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.Maui.Controls.BindableProperty ModeProperty =global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(propertyName: "Mode",
returnType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode),
declaringType: typeof(TreeViewExtensions),
defaultValue: (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)1,
defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay,
validateValue: null,
propertyChanged: static (sender, oldValue, newValue) =>
{
OnModeChanged();
OnModeChanged((global::Microsoft.Maui.Controls.VisualElement)sender);
OnModeChanged((global::Microsoft.Maui.Controls.VisualElement)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)newValue);
OnModeChanged((global::Microsoft.Maui.Controls.VisualElement)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)oldValue, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)newValue);
},
propertyChanging: static (sender, oldValue, newValue) =>
{
OnModeChanging();
OnModeChanging((global::Microsoft.Maui.Controls.VisualElement)sender);
OnModeChanging((global::Microsoft.Maui.Controls.VisualElement)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)newValue);
OnModeChanging((global::Microsoft.Maui.Controls.VisualElement)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)oldValue, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)newValue);
},
coerceValue: null,
defaultValueCreator: null);
/// <summary>
/// Default value: Mode2
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetMode(global::Microsoft.Maui.Controls.VisualElement element, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(ModeProperty, value);
}
/// <summary>
/// Default value: Mode2
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode GetMode(global::Microsoft.Maui.Controls.VisualElement element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode)element.GetValue(ModeProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanged(global::Microsoft.Maui.Controls.VisualElement visualElement);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanged(global::Microsoft.Maui.Controls.VisualElement visualElement, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanged(global::Microsoft.Maui.Controls.VisualElement visualElement, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanging();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanging(global::Microsoft.Maui.Controls.VisualElement visualElement);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanging(global::Microsoft.Maui.Controls.VisualElement visualElement, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnModeChanging(global::Microsoft.Maui.Controls.VisualElement visualElement, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Mode newValue);
}
}
