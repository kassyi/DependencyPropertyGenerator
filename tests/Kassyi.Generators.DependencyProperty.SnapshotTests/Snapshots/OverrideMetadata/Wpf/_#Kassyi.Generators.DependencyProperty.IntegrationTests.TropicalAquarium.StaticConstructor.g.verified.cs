//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium.StaticConstructor.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class TropicalAquarium
{
static TropicalAquarium()
{
AquariumSizeProperty.OverrideMetadata(forType: typeof(TropicalAquarium), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: (int)20,
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((TropicalAquarium)sender).OnAquariumSizeChanged();
((TropicalAquarium)sender).OnAquariumSizeChanged((int)args.NewValue);
((TropicalAquarium)sender).OnAquariumSizeChanged((int)args.OldValue, (int)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false));
}


[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int oldValue, int newValue);
}
}
