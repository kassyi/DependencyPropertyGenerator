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
public static readonly global::Microsoft.UI.Xaml.DependencyProperty SomePropertyProperty =global::Microsoft.UI.Xaml.DependencyProperty.RegisterAttached(name: "SomeProperty",
propertyType: typeof(object),
ownerType: typeof(GridExtensions),
new global::Microsoft.UI.Xaml.PropertyMetadata(
    defaultValue: default(object),
    propertyChangedCallback: static (sender, args) =>
{
OnSomePropertyChanged();
OnSomePropertyChanged((global::Microsoft.UI.Xaml.DependencyObject)sender);
OnSomePropertyChanged((global::Microsoft.UI.Xaml.DependencyObject)sender, (object?)args.NewValue);
OnSomePropertyChanged((global::Microsoft.UI.Xaml.DependencyObject)sender, (object?)args.OldValue, (object?)args.NewValue);
}));
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetSomeProperty(global::Microsoft.UI.Xaml.DependencyObject element, object? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(SomePropertyProperty, value);
}
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static object? GetSomeProperty(global::Microsoft.UI.Xaml.DependencyObject element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (object?)element.GetValue(SomePropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged(global::Microsoft.UI.Xaml.DependencyObject dependencyObject);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged(global::Microsoft.UI.Xaml.DependencyObject dependencyObject, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSomePropertyChanged(global::Microsoft.UI.Xaml.DependencyObject dependencyObject, object? oldValue, object? newValue);
}
}
