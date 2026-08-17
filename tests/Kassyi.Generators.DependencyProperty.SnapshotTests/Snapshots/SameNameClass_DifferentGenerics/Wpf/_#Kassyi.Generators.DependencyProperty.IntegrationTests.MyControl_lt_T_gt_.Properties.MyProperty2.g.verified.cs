//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl_lt_T_gt_.Properties.MyProperty2.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl<T>
{
/// <summary>
/// Identifies the <see cref="MyProperty2"/> dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty MyProperty2Property =
global::System.Windows.DependencyProperty.Register(name: "MyProperty2",
propertyType: typeof(string),
ownerType: typeof(MyControl<T>),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(string),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl<T>)sender).OnMyProperty2Changed();
((MyControl<T>)sender).OnMyProperty2Changed((string?)args.NewValue);
((MyControl<T>)sender).OnMyProperty2Changed((string?)args.OldValue, (string?)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string? MyProperty2
{
get => (string?)GetValue(MyProperty2Property);
set => SetValue(MyProperty2Property, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyProperty2Changed();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyProperty2Changed(string? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyProperty2Changed(string? oldValue, string? newValue);
}
}
