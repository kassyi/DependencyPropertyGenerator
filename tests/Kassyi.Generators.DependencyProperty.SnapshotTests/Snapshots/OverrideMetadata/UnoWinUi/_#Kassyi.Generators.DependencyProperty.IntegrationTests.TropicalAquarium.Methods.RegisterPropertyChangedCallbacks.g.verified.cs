//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium.Methods.RegisterPropertyChangedCallbacks.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class TropicalAquarium
    {
        private void RegisterPropertyChangedCallbacks()
        {
            _ = this.RegisterPropertyChangedCallback(dp: AquariumGraphicProperty, callback: static (sender, dependencyProperty) =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium)sender).OnAquariumGraphicChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium)sender).OnAquariumGraphicChanged((global::System.Uri)sender.GetValue(dependencyProperty));
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium)sender).OnAquariumGraphicChanged((global::System.Uri)sender.GetValue(dependencyProperty), (global::System.Uri)sender.GetValue(dependencyProperty));
            );
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged(global::System.Uri newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged(global::System.Uri oldValue, global::System.Uri newValue);
    }
}