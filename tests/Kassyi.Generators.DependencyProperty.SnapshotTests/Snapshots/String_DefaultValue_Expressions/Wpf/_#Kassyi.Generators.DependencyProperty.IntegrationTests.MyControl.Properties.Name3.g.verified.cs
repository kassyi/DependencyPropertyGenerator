//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.Name3.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="Name3"/> dependency property.<br/>
/// Default value: Empty"
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty Name3Property =
global::System.Windows.DependencyProperty.Register(name: "Name3",
propertyType: typeof(string),
ownerType: typeof(MyControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: (string)"string.Empty",
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnName3Changed();
((MyControl)sender).OnName3Changed((string)args.NewValue);
((MyControl)sender).OnName3Changed((string)args.OldValue, (string)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: Empty"
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string Name3
{
get => (string)GetValue(Name3Property);
set => SetValue(Name3Property, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnName3Changed();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnName3Changed(string newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnName3Changed(string oldValue, string newValue);
}
}
