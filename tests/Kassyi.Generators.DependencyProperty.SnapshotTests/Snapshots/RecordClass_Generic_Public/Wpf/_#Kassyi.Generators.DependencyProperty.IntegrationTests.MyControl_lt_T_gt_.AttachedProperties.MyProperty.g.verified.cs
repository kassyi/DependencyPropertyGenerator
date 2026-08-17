//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl_lt_T_gt_.AttachedProperties.MyProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial record MyControl<T>
{
/// <summary>
/// Identifies the MyProperty dependency property.<br/>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty MyPropertyProperty =global::System.Windows.DependencyProperty.RegisterAttached(name: "MyProperty",
propertyType: typeof(object),
ownerType: typeof(MyControl<T>),
defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(object),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
OnMyPropertyChanged();
OnMyPropertyChanged((global::System.Windows.DependencyObject)sender);
OnMyPropertyChanged((global::System.Windows.DependencyObject)sender, (object?)args.NewValue);
OnMyPropertyChanged((global::System.Windows.DependencyObject)sender, (object?)args.OldValue, (object?)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetMyProperty(global::System.Windows.DependencyObject element, object? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(MyPropertyProperty, value);
}
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.Windows.AttachedPropertyBrowsableForType(typeof(global::System.Windows.DependencyObject))]
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static object? GetMyProperty(global::System.Windows.DependencyObject element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (object?)element.GetValue(MyPropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged(global::System.Windows.DependencyObject dependencyObject);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged(global::System.Windows.DependencyObject dependencyObject, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged(global::System.Windows.DependencyObject dependencyObject, object? oldValue, object? newValue);
}
}
