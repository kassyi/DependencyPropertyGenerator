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
        internal static readonly global::System.Windows.DependencyPropertyKey IsSpinningPropertyKey = global::System.Windows.DependencyProperty.RegisterReadOnly(name: "IsSpinning", propertyType: typeof(bool), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(bool), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: null, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Identifies the <see cref = "IsSpinning"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        public static readonly global::System.Windows.DependencyProperty IsSpinningProperty = IsSpinningPropertyKey.DependencyProperty;
        /// <summary>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool IsSpinning { get => (bool)GetValue(IsSpinningProperty); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool oldValue, bool newValue);
    }
}