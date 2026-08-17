//HintName: MyNamespace.OuterClass_lt_T_gt_.InnerControl.Properties.MyProperty.g.cs

#nullable enable

namespace MyNamespace
{
internal partial class OuterClass<T>
{
internal partial class InnerControl
{
/// <summary>
/// Identifies the <see cref="MyProperty"/> dependency property.<br/>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty MyPropertyProperty =
global::System.Windows.DependencyProperty.Register(name: "MyProperty",
propertyType: typeof(int),
ownerType: typeof(InnerControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(int),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((InnerControl)sender).OnMyPropertyChanged();
((InnerControl)sender).OnMyPropertyChanged((int)args.NewValue);
((InnerControl)sender).OnMyPropertyChanged((int)args.OldValue, (int)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public int MyProperty
{
get => (int)GetValue(MyPropertyProperty);
set => SetValue(MyPropertyProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyChanged(int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnMyPropertyChanged(int oldValue, int newValue);
}
}
}
