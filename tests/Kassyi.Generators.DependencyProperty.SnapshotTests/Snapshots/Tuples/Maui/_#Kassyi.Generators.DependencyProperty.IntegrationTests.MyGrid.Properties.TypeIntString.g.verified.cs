//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.TypeIntString.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "TypeIntString"/> dependency property.<br/>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty TypeIntStringProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "TypeIntString", returnType: typeof((int, string)), declaringType: typeof(MyGrid), defaultValue: default((int, string)), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnTypeIntStringChanged();
            ((MyGrid)sender).OnTypeIntStringChanged(((int, string))newValue);
            ((MyGrid)sender).OnTypeIntStringChanged(((int, string))oldValue, ((int, string))newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnTypeIntStringChanging();
            ((MyGrid)sender).OnTypeIntStringChanging(((int, string))newValue);
            ((MyGrid)sender).OnTypeIntStringChanging(((int, string))oldValue, ((int, string))newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public (int, string) TypeIntString { get => ((int, string))GetValue(TypeIntStringProperty); set => SetValue(TypeIntStringProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanged((int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanged((int, string) oldValue, (int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanging((int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanging((int, string) oldValue, (int, string) newValue);
    }
}