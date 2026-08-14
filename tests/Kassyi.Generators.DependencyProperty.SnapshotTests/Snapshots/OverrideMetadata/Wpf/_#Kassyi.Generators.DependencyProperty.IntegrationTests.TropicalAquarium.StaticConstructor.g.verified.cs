//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium.StaticConstructor.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class TropicalAquarium
    {
        static TropicalAquarium()
        {
            AquariumGraphicProperty.OverrideMetadata(forType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: (global::System.Uri)new System.Uri("http://www.contoso.com/tropical-aquarium-graphic.jpg"), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
            {
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium)sender).OnAquariumGraphicChanged();
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium)sender).OnAquariumGraphicChanged((global::System.Uri)args.NewValue);
                ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.TropicalAquarium)sender).OnAquariumGraphicChanged((global::System.Uri)args.OldValue, (global::System.Uri)args.NewValue);
            }, coerceValueCallback: null, isAnimationProhibited: false));
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged(global::System.Uri newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged(global::System.Uri oldValue, global::System.Uri newValue);
    }
}