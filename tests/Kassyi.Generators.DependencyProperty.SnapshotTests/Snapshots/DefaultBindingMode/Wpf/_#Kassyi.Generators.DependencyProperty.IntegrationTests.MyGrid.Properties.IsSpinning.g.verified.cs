//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.IsSpinning.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "IsSpinning"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty IsSpinningProperty = global::System.Windows.DependencyProperty.Register(name: "IsSpinning", propertyType: typeof(bool), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(bool), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanged((bool)args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanged((bool)args.OldValue, (bool)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool IsSpinning { get => (bool)GetValue(IsSpinningProperty); set => SetValue(IsSpinningProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool oldValue, bool newValue);
    }
}