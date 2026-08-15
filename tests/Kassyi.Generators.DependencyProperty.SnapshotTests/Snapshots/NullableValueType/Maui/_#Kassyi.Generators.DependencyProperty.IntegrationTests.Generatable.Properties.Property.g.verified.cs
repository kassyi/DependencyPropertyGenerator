//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.Properties.Property.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class Generatable
    {
        /// <summary>
        /// Identifies the <see cref = "Property"/> dependency property.<br/>
        /// Default value: default(int?)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty PropertyProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "Property", returnType: typeof(int? ), declaringType: typeof(Generatable), defaultValue: default(int? ), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((Generatable)sender).OnPropertyChanged();
            ((Generatable)sender).OnPropertyChanged((int? )newValue);
            ((Generatable)sender).OnPropertyChanged((int? )oldValue, (int? )newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((Generatable)sender).OnPropertyChanging();
            ((Generatable)sender).OnPropertyChanging((int? )newValue);
            ((Generatable)sender).OnPropertyChanging((int? )oldValue, (int? )newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default(int?)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int? Property { get => (int? )GetValue(PropertyProperty); set => SetValue(PropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanged(int? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanged(int? oldValue, int? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanging(int? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnPropertyChanging(int? oldValue, int? newValue);
    }
}