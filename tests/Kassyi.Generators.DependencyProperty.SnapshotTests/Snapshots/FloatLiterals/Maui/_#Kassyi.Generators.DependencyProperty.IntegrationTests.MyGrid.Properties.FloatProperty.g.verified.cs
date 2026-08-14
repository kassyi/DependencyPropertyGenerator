//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.FloatProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "FloatProperty"/> dependency property.<br/>
        /// Default value: 42
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty FloatPropertyProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "FloatProperty", returnType: typeof(float), declaringType: typeof(MyGrid), defaultValue: (float)42, defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnFloatPropertyChanged();
            ((MyGrid)sender).OnFloatPropertyChanged((float)newValue);
            ((MyGrid)sender).OnFloatPropertyChanged((float)oldValue, (float)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnFloatPropertyChanging();
            ((MyGrid)sender).OnFloatPropertyChanging((float)newValue);
            ((MyGrid)sender).OnFloatPropertyChanging((float)oldValue, (float)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: 42
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public float FloatProperty { get => (float)GetValue(FloatPropertyProperty); set => SetValue(FloatPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanged(float newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanged(float oldValue, float newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanging(float newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanging(float oldValue, float newValue);
    }
}