//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.AttachedProperties.AttachedReadOnlyProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class GridExtensions
{
/// <summary>
/// Identifies the AttachedReadOnlyProperty dependency property.<br/>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.UI.Xaml.DependencyProperty AttachedReadOnlyPropertyProperty =global::Microsoft.UI.Xaml.DependencyProperty.RegisterAttached(name: "AttachedReadOnlyProperty",
propertyType: typeof(object),
ownerType: typeof(GridExtensions),
new global::Microsoft.UI.Xaml.PropertyMetadata(
    defaultValue: default(object),
    propertyChangedCallback: static (sender, args) =>
{
OnAttachedReadOnlyPropertyChanged();
OnAttachedReadOnlyPropertyChanged((global::Microsoft.UI.Xaml.Controls.Grid)sender);
OnAttachedReadOnlyPropertyChanged((global::Microsoft.UI.Xaml.Controls.Grid)sender, (object?)args.NewValue);
OnAttachedReadOnlyPropertyChanged((global::Microsoft.UI.Xaml.Controls.Grid)sender, (object?)args.OldValue, (object?)args.NewValue);
}));
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static void SetAttachedReadOnlyProperty(global::Microsoft.UI.Xaml.Controls.Grid element, object? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(AttachedReadOnlyPropertyProperty, value);
}
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static object? GetAttachedReadOnlyProperty(global::Microsoft.UI.Xaml.Controls.Grid element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (object?)element.GetValue(AttachedReadOnlyPropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedReadOnlyPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedReadOnlyPropertyChanged(global::Microsoft.UI.Xaml.Controls.Grid grid);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedReadOnlyPropertyChanged(global::Microsoft.UI.Xaml.Controls.Grid grid, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnAttachedReadOnlyPropertyChanged(global::Microsoft.UI.Xaml.Controls.Grid grid, object? oldValue, object? newValue);
}
}
