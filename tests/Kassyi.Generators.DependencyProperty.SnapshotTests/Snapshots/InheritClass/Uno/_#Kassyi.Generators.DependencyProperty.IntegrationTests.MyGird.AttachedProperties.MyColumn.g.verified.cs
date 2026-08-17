//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGird.AttachedProperties.MyColumn.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyGird
{
/// <summary>
/// Identifies the MyColumn dependency property.<br/>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Windows.UI.Xaml.DependencyProperty MyColumnProperty =global::Windows.UI.Xaml.DependencyProperty.RegisterAttached(name: "MyColumn",
propertyType: typeof(int),
ownerType: typeof(MyGird),
new global::Windows.UI.Xaml.PropertyMetadata(
    defaultValue: default(int),
    propertyChangedCallback: static (sender, args) =>
{
OnMyColumnChanged();
OnMyColumnChanged((global::Windows.UI.Xaml.FrameworkElement)sender);
OnMyColumnChanged((global::Windows.UI.Xaml.FrameworkElement)sender, (int)args.NewValue);
OnMyColumnChanged((global::Windows.UI.Xaml.FrameworkElement)sender, (int)args.OldValue, (int)args.NewValue);
}));
/// <summary>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetMyColumn(global::Windows.UI.Xaml.FrameworkElement element, int value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(MyColumnProperty, value);
}
/// <summary>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static int GetMyColumn(global::Windows.UI.Xaml.FrameworkElement element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (int)element.GetValue(MyColumnProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyColumnChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyColumnChanged(global::Windows.UI.Xaml.FrameworkElement frameworkElement);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyColumnChanged(global::Windows.UI.Xaml.FrameworkElement frameworkElement, int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyColumnChanged(global::Windows.UI.Xaml.FrameworkElement frameworkElement, int oldValue, int newValue);
}
}
