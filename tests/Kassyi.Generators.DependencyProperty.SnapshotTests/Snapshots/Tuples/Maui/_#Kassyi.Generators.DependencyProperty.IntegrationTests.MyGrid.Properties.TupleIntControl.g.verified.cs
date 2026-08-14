//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.TupleIntControl.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "TupleIntControl"/> dependency property.<br/>
        /// Default value: default(Tuple&lt;int, VisualElement&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty TupleIntControlProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "TupleIntControl", returnType: typeof(global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>), declaringType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid), defaultValue: default(global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: null, propertyChanging: null, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default(Tuple&lt;int, VisualElement&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? TupleIntControl { get => (global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? )GetValue(TupleIntControlProperty); set => SetValue(TupleIntControlProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanged(global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanged(global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? oldValue, global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanging(global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanging(global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? oldValue, global::System.Tuple<int, global::Microsoft.Maui.Controls.VisualElement>? newValue);
    }
}