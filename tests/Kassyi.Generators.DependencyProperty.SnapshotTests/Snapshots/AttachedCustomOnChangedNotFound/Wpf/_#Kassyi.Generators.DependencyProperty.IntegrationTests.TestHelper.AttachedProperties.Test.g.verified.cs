//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TestHelper.AttachedProperties.Test.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class TestHelper
{
/// <summary>
/// Identifies the Test dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty TestProperty =global::System.Windows.DependencyProperty.RegisterAttached(name: "Test",
propertyType: typeof(string),
ownerType: typeof(TestHelper),
defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(string),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: null,
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);
/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetTest(global::System.Windows.DependencyObject element, string? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(TestProperty, value);
}
/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.Windows.AttachedPropertyBrowsableForType(typeof(global::System.Windows.DependencyObject))]
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static string? GetTest(global::System.Windows.DependencyObject element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (string?)element.GetValue(TestProperty);
}
#error DPG0001: The specified OnChanged method 'NonExistentMethod' was not found or has an unsupported signature on 'Kassyi.Generators.DependencyProperty.IntegrationTests.TestHelper'.
}
}
