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
public static readonly global::System.Windows.DependencyProperty AquariumSizeProperty =
global::System.Windows.DependencyProperty.Register(name: "AquariumSize",
propertyType: typeof(int),
ownerType: typeof(Aquarium),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: (int)10,
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.AffectsRender,
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
/// Default value: 10
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public int AquariumSize
{
get => (int)GetValue(AquariumSizeProperty);
set => SetValue(AquariumSizeProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int oldValue, int newValue);
}
}
