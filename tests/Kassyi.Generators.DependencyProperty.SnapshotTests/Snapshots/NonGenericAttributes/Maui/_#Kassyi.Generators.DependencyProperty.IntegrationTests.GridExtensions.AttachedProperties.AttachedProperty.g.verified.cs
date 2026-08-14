//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.AttachedProperties.AttachedProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public static partial class GridExtensions
    {
        /// <summary>
        /// Identifies the AttachedProperty dependency property.<br/>
        /// Default value: default(object)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty AttachedPropertyProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(propertyName: "AttachedProperty", returnType: typeof(object), declaringType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions), defaultValue: default(object), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            OnAttachedPropertyChanged();
            OnAttachedPropertyChanged((global::Microsoft.Maui.Controls.Grid)sender);
            OnAttachedPropertyChanged((global::Microsoft.Maui.Controls.Grid)sender, (object? )newValue);
            OnAttachedPropertyChanged((global::Microsoft.Maui.Controls.Grid)sender, (object? )oldValue, (object? )newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            OnAttachedPropertyChanging();
            OnAttachedPropertyChanging((global::Microsoft.Maui.Controls.Grid)sender);
            OnAttachedPropertyChanging((global::Microsoft.Maui.Controls.Grid)sender, (object? )newValue);
            OnAttachedPropertyChanging((global::Microsoft.Maui.Controls.Grid)sender, (object? )oldValue, (object? )newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default(object)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetAttachedProperty(global::Microsoft.Maui.Controls.Grid element, object? value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(AttachedPropertyProperty, value);
        }

        /// <summary>
        /// Default value: default(object)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static object? GetAttachedProperty(global::Microsoft.Maui.Controls.Grid element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (object? )element.GetValue(AttachedPropertyProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanged(global::Microsoft.Maui.Controls.Grid grid);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanged(global::Microsoft.Maui.Controls.Grid grid, object? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanged(global::Microsoft.Maui.Controls.Grid grid, object? oldValue, object? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanging(global::Microsoft.Maui.Controls.Grid grid);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanging(global::Microsoft.Maui.Controls.Grid grid, object? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnAttachedPropertyChanging(global::Microsoft.Maui.Controls.Grid grid, object? oldValue, object? newValue);
    }
}