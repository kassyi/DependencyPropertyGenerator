//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.TypeControlInt.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "TypeControlInt"/> dependency property.<br/>
        /// Default value: default((VisualElement, int))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty TypeControlIntProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "TypeControlInt", returnType: typeof((global::Microsoft.Maui.Controls.VisualElement, int)), declaringType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid), defaultValue: default((global::Microsoft.Maui.Controls.VisualElement, int)), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnTypeControlIntChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnTypeControlIntChanged(((global::Microsoft.Maui.Controls.VisualElement, int))newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnTypeControlIntChanged(((global::Microsoft.Maui.Controls.VisualElement, int))oldValue, ((global::Microsoft.Maui.Controls.VisualElement, int))newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnTypeControlIntChanging();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnTypeControlIntChanging(((global::Microsoft.Maui.Controls.VisualElement, int))newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnTypeControlIntChanging(((global::Microsoft.Maui.Controls.VisualElement, int))oldValue, ((global::Microsoft.Maui.Controls.VisualElement, int))newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default((VisualElement, int))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public (global::Microsoft.Maui.Controls.VisualElement, int) TypeControlInt { get => ((global::Microsoft.Maui.Controls.VisualElement, int))GetValue(TypeControlIntProperty); set => SetValue(TypeControlIntProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanged((global::Microsoft.Maui.Controls.VisualElement, int) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanged((global::Microsoft.Maui.Controls.VisualElement, int) oldValue, (global::Microsoft.Maui.Controls.VisualElement, int) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanging((global::Microsoft.Maui.Controls.VisualElement, int) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanging((global::Microsoft.Maui.Controls.VisualElement, int) oldValue, (global::Microsoft.Maui.Controls.VisualElement, int) newValue);
    }
}