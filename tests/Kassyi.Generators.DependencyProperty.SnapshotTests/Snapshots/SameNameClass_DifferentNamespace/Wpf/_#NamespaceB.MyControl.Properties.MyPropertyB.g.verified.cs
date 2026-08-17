//HintName: NamespaceB.MyControl.Properties.MyPropertyB.g.cs

#nullable enable

namespace NamespaceB
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="MyPropertyB"/> dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty MyPropertyBProperty =
global::System.Windows.DependencyProperty.Register(name: "MyPropertyB",
propertyType: typeof(string),
ownerType: typeof(MyControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(string),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnMyPropertyBChanged();
((MyControl)sender).OnMyPropertyBChanged((string?)args.NewValue);
((MyControl)sender).OnMyPropertyBChanged((string?)args.OldValue, (string?)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string? MyPropertyB
{
get => (string?)GetValue(MyPropertyBProperty);
set => SetValue(MyPropertyBProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyBChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyBChanged(string? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyBChanged(string? oldValue, string? newValue);
}
}
