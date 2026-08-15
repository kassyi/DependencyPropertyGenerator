//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl.Properties.MyProperty2.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class AnotherControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty2"/> dependency property.<br/>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty MyProperty2Property = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "MyProperty2", returnType: typeof((int, string)), declaringType: typeof(AnotherControl), defaultValue: default((int, string)), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((AnotherControl)sender).OnMyProperty2Changed();
            ((AnotherControl)sender).OnMyProperty2Changed(((int, string))newValue);
            ((AnotherControl)sender).OnMyProperty2Changed(((int, string))oldValue, ((int, string))newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((AnotherControl)sender).OnMyProperty2Changing();
            ((AnotherControl)sender).OnMyProperty2Changing(((int, string))newValue);
            ((AnotherControl)sender).OnMyProperty2Changing(((int, string))oldValue, ((int, string))newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public (int, string) MyProperty2 { get => ((int, string))GetValue(MyProperty2Property); set => SetValue(MyProperty2Property, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed((int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed((int, string) oldValue, (int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changing();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changing((int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changing((int, string) oldValue, (int, string) newValue);
    }
}