//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium.Properties.AquariumSize.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class Aquarium
{
/// <summary>
/// Identifies the <see cref="AquariumSize"/> dependency property.<br/>
/// Default value: 10
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
internal static readonly global::System.Windows.DependencyPropertyKey AquariumSizePropertyKey =
global::System.Windows.DependencyProperty.RegisterReadOnly(name: "AquariumSize",
propertyType: typeof(int),
ownerType: typeof(Aquarium),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: (int)10,
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((Aquarium)sender).OnAquariumSizeChanged();
((Aquarium)sender).OnAquariumSizeChanged((int)args.NewValue);
((Aquarium)sender).OnAquariumSizeChanged((int)args.OldValue, (int)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Identifies the <see cref="AquariumSize"/> dependency property.<br/>
/// Default value: 10
/// </summary>
public static readonly global::System.Windows.DependencyProperty AquariumSizeProperty
= AquariumSizePropertyKey.DependencyProperty;
/// <summary>
/// Default value: 10
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public int AquariumSize
{
get => (int)GetValue(AquariumSizeProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int oldValue, int newValue);
}
}
