//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Test.AttachedProperties.TestProp.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class Test
{
/// <summary>
/// Identifies the TestProp dependency property.<br/>
/// Default value: default(Test)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Windows.UI.Xaml.DependencyProperty TestPropProperty =global::Windows.UI.Xaml.DependencyProperty.RegisterAttached(name: "TestProp",
propertyType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test),
ownerType: typeof(Test),
new global::Windows.UI.Xaml.PropertyMetadata(
    defaultValue: default(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test),
    propertyChangedCallback: static (sender, args) =>
{
TestChanged((global::Windows.UI.Xaml.Controls.Grid)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test?)args.NewValue);
}));
/// <summary>
/// Default value: default(Test)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetTestProp(global::Windows.UI.Xaml.Controls.Grid element, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(TestPropProperty, value);
}
/// <summary>
/// Default value: default(Test)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? GetTestProp(global::Windows.UI.Xaml.Controls.Grid element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test?)element.GetValue(TestPropProperty);
}
}
}
