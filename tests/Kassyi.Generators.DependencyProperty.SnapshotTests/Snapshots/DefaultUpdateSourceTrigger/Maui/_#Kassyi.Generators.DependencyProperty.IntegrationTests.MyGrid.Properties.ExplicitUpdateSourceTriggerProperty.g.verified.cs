//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.ExplicitUpdateSourceTriggerProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "ExplicitUpdateSourceTriggerProperty"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty ExplicitUpdateSourceTriggerPropertyProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "ExplicitUpdateSourceTriggerProperty", returnType: typeof(bool), declaringType: typeof(MyGrid), defaultValue: default(bool), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnExplicitUpdateSourceTriggerPropertyChanged();
            ((MyGrid)sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)newValue);
            ((MyGrid)sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)oldValue, (bool)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnExplicitUpdateSourceTriggerPropertyChanging();
            ((MyGrid)sender).OnExplicitUpdateSourceTriggerPropertyChanging((bool)newValue);
            ((MyGrid)sender).OnExplicitUpdateSourceTriggerPropertyChanging((bool)oldValue, (bool)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool ExplicitUpdateSourceTriggerProperty { get => (bool)GetValue(ExplicitUpdateSourceTriggerPropertyProperty); set => SetValue(ExplicitUpdateSourceTriggerPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanged(bool oldValue, bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanging(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanging(bool oldValue, bool newValue);
    }
}