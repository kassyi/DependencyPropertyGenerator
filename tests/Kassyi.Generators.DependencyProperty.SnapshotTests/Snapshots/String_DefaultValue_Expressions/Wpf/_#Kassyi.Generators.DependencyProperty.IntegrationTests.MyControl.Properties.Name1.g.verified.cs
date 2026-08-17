//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.Name1.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="Name1"/> dependency property.<br/>
/// Default value: "nameof(MyControl)"
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty Name1Property =
global::System.Windows.DependencyProperty.Register(name: "Name1",
propertyType: typeof(string),
ownerType: typeof(MyControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: (string)"nameof(MyControl)",
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnName1Changed();
((MyControl)sender).OnName1Changed((string)args.NewValue);
((MyControl)sender).OnName1Changed((string)args.OldValue, (string)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: "nameof(MyControl)"
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string Name1
{
get => (string)GetValue(Name1Property);
set => SetValue(Name1Property, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnName1Changed();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnName1Changed(string newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnName1Changed(string oldValue, string newValue);
}
}
