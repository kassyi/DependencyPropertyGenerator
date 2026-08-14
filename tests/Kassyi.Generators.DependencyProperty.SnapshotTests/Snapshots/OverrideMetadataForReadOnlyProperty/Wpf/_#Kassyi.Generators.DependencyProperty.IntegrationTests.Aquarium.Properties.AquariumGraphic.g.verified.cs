//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium.Properties.AquariumGraphic.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Aquarium
    {
        /// <summary>
        /// Identifies the <see cref = "AquariumGraphic"/> dependency property.<br/>
        /// Default value: jpg")
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        internal static readonly global::System.Windows.DependencyPropertyKey AquariumGraphicPropertyKey = global::System.Windows.DependencyProperty.RegisterReadOnly(name: "AquariumGraphic", propertyType: typeof(global::System.Uri), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: (global::System.Uri)new System.Uri("http://www.contoso.com/aquarium-graphic.jpg"), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium)sender).OnAquariumGraphicChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium)sender).OnAquariumGraphicChanged((global::System.Uri)args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Aquarium)sender).OnAquariumGraphicChanged((global::System.Uri)args.OldValue, (global::System.Uri)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Identifies the <see cref = "AquariumGraphic"/> dependency property.<br/>
        /// Default value: jpg")
        /// </summary>
        public static readonly global::System.Windows.DependencyProperty AquariumGraphicProperty = AquariumGraphicPropertyKey.DependencyProperty;
        /// <summary>
        /// Default value: jpg")
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Uri AquariumGraphic { get => (global::System.Uri)GetValue(AquariumGraphicProperty); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged(global::System.Uri newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnAquariumGraphicChanged(global::System.Uri oldValue, global::System.Uri newValue);
    }
}