//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.class.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="class"/> dependency property.<br/>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty classProperty =
global::System.Windows.DependencyProperty.Register(name: "class",
propertyType: typeof(int),
ownerType: typeof(MyControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(int),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnclassChanged();
((MyControl)sender).OnclassChanged((int)args.NewValue);
((MyControl)sender).OnclassChanged((int)args.OldValue, (int)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: default(int)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public int @class
{
get => (int)GetValue(classProperty);
set => SetValue(classProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnclassChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnclassChanged(int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnclassChanged(int oldValue, int newValue);
}
}
