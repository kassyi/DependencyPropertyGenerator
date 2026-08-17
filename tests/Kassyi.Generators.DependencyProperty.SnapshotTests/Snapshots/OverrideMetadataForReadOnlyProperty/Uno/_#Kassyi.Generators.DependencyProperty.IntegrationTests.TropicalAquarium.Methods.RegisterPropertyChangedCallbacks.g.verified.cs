//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium.Methods.RegisterPropertyChangedCallbacks.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class TropicalAquarium
{
private void RegisterPropertyChangedCallbacks()
{
_ = this.RegisterPropertyChangedCallback(dp: AquariumSizeProperty, callback: static (sender, dependencyProperty) =>
{
((TropicalAquarium)sender).OnAquariumSizeChanged();
((TropicalAquarium)sender).OnAquariumSizeChanged((int)sender.GetValue(dependencyProperty));
((TropicalAquarium)sender).OnAquariumSizeChanged(default(int), (int)sender.GetValue(dependencyProperty));
});
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnAquariumSizeChanged(int oldValue, int newValue);
}
}
