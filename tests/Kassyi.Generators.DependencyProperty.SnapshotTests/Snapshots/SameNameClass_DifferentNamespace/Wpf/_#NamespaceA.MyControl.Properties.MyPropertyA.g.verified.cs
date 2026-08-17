//HintName: NamespaceA.MyControl.Properties.MyPropertyA.g.cs

#nullable enable

namespace NamespaceA
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="MyPropertyA"/> dependency property.<br/>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty MyPropertyAProperty =
global::System.Windows.DependencyProperty.Register(name: "MyPropertyA",
propertyType: typeof(int),
ownerType: typeof(MyControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(int),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnMyPropertyAChanged();
((MyControl)sender).OnMyPropertyAChanged((int)args.NewValue);
((MyControl)sender).OnMyPropertyAChanged((int)args.OldValue, (int)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public int MyPropertyA
{
get => (int)GetValue(MyPropertyAProperty);
set => SetValue(MyPropertyAProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyAChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyAChanged(int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyAChanged(int oldValue, int newValue);
}
}
