//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControlHelper.AttachedProperties.AttachedNotNullStringProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public static partial class MyControlHelper
    {
        /// <summary>
        /// Identifies the AttachedNotNullStringProperty dependency property.<br/>
        /// Default value: ""
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty AttachedNotNullStringPropertyProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(propertyName: "AttachedNotNullStringProperty", returnType: typeof(string), declaringType: typeof(MyControlHelper), defaultValue: (string)"", defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: static (sender, value) => IsAttachedNotNullStringPropertyValid((global::Microsoft.Maui.Controls.Grid)sender, (string? )value), propertyChanged: static (sender, oldValue, newValue) =>
        {
            OnAttachedNotNullStringPropertyChanged();
            OnAttachedNotNullStringPropertyChanged((global::Microsoft.Maui.Controls.Grid)sender);
            OnAttachedNotNullStringPropertyChanged((global::Microsoft.Maui.Controls.Grid)sender, (string)newValue);
            OnAttachedNotNullStringPropertyChanged((global::Microsoft.Maui.Controls.Grid)sender, (string)oldValue, (string)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            OnAttachedNotNullStringPropertyChanging();
            OnAttachedNotNullStringPropertyChanging((global::Microsoft.Maui.Controls.Grid)sender);
            OnAttachedNotNullStringPropertyChanging((global::Microsoft.Maui.Controls.Grid)sender, (string)newValue);
            OnAttachedNotNullStringPropertyChanging((global::Microsoft.Maui.Controls.Grid)sender, (string)oldValue, (string)newValue);
        }, coerceValue: static (sender, value) => CoerceAttachedNotNullStringProperty((global::Microsoft.Maui.Controls.Grid)sender, (string? )value), defaultValueCreator: null);
        /// <summary>
        /// Default value: ""
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetAttachedNotNullStringProperty(global::Microsoft.Maui.Controls.Grid element, string value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(AttachedNotNullStringPropertyProperty, value);
        }

        /// <summary>
        /// Default value: ""
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static string GetAttachedNotNullStringProperty(global::Microsoft.Maui.Controls.Grid element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (string)element.GetValue(AttachedNotNullStringPropertyProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged(global::Microsoft.Maui.Controls.Grid grid);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged(global::Microsoft.Maui.Controls.Grid grid, string newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanged(global::Microsoft.Maui.Controls.Grid grid, string oldValue, string newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanging(global::Microsoft.Maui.Controls.Grid grid);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanging(global::Microsoft.Maui.Controls.Grid grid, string newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedNotNullStringPropertyChanging(global::Microsoft.Maui.Controls.Grid grid, string oldValue, string newValue);
        private static partial string CoerceAttachedNotNullStringProperty(global::Microsoft.Maui.Controls.Grid grid, string? value);
        private static partial bool IsAttachedNotNullStringPropertyValid(global::Microsoft.Maui.Controls.Grid sender, string? value);
    }
}