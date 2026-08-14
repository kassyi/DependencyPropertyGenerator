//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl.Properties.MyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class AnotherControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty MyPropertyProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "MyProperty", returnType: typeof(int), declaringType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl), defaultValue: default(int), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanged((int)newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanged((int)oldValue, (int)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanging();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanging((int)newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanging((int)oldValue, (int)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int MyProperty { get => (int)GetValue(MyPropertyProperty); set => SetValue(MyPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(int oldValue, int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanging(int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanging(int oldValue, int newValue);
    }
}