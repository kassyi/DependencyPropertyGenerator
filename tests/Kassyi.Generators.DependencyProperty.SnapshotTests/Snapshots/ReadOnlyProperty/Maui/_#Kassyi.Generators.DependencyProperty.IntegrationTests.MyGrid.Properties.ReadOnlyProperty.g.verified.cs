//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.ReadOnlyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "ReadOnlyProperty"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindablePropertyKey ReadOnlyPropertyPropertyKey = global::Microsoft.Maui.Controls.BindableProperty.CreateReadOnly(propertyName: "ReadOnlyProperty", returnType: typeof(bool), declaringType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid), defaultValue: default(bool), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWayToSource, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnReadOnlyPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnReadOnlyPropertyChanged((bool)newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnReadOnlyPropertyChanged((bool)oldValue, (bool)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnReadOnlyPropertyChanging();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnReadOnlyPropertyChanging((bool)newValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid)sender).OnReadOnlyPropertyChanging((bool)oldValue, (bool)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Identifies the <see cref = "ReadOnlyProperty"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        public static readonly global::Microsoft.Maui.Controls.BindableProperty ReadOnlyPropertyProperty = ReadOnlyPropertyPropertyKey.BindableProperty;
        /// <summary>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool ReadOnlyProperty { get => (bool)GetValue(ReadOnlyPropertyProperty); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanged(bool oldValue, bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanging(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanging(bool oldValue, bool newValue);
    }
}