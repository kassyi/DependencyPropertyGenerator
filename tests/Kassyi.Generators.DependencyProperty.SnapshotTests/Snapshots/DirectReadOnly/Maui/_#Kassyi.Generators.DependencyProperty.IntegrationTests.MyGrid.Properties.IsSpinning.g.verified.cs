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
        public static readonly global::Microsoft.Maui.Controls.BindablePropertyKey IsSpinningPropertyKey = global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly(propertyName: "IsSpinning", returnType: typeof(bool), declaringType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid), defaultValue: default(bool), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWayToSource, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanged((bool)newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanged((bool)oldValue, (bool)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanging();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanging((bool)newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnIsSpinningChanging((bool)oldValue, (bool)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Identifies the <see cref = "IsSpinning"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        public static readonly global::Microsoft.Maui.Controls.BindableProperty IsSpinningProperty = IsSpinningPropertyKey.BindableProperty;
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
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanging(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanging(bool oldValue, bool newValue);
    }
}