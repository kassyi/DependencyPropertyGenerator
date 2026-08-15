//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.MyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty MyPropertyProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "MyProperty", returnType: typeof(int), declaringType: typeof(MyGrid), defaultValue: default(int), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnMyPropertyChanged();
            ((MyGrid)sender).OnMyPropertyChanged((int)newValue);
            ((MyGrid)sender).OnMyPropertyChanged((int)oldValue, (int)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((MyGrid)sender).OnMyPropertyChanging();
            ((MyGrid)sender).OnMyPropertyChanging((int)newValue);
            ((MyGrid)sender).OnMyPropertyChanging((int)oldValue, (int)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public required partial int MyProperty { get => (int)GetValue(MyPropertyProperty); init => SetValue(MyPropertyProperty, value); }

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