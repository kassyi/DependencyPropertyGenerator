//HintName: MyNamespace.MyControl.Properties.MyProperty.g.cs

#nullable enable

namespace MyNamespace
{
internal partial class MyControl
{
/// <summary>
/// Identifies the <see cref="MyProperty"/> dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty MyPropertyProperty =
global::System.Windows.DependencyProperty.Register(name: "MyProperty",
propertyType: typeof(string),
ownerType: typeof(MyControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(string),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnMyPropertyChanged();
((MyControl)sender).OnMyPropertyChanged((string?)args.NewValue);
((MyControl)sender).OnMyPropertyChanged((string?)args.OldValue, (string?)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public string? MyProperty
{
get => (string?)GetValue(MyPropertyProperty);
set => SetValue(MyPropertyProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyChanged(string? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyChanged(string? oldValue, string? newValue);
}
}
