//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridHelpers.AttachedProperties.RowCount.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public static partial class GridHelpers
    {
        /// <summary>
        /// Identifies the RowCount dependency property.<br/>
        /// Default value: -1
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty RowCountProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(propertyName: "RowCount", returnType: typeof(int), declaringType: typeof(GridHelpers), defaultValue: (int)-1, defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            OnRowCountChanged((global::Microsoft.Maui.Controls.Grid)sender, (int)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            OnRowCountChanging();
            OnRowCountChanging((global::Microsoft.Maui.Controls.Grid)sender);
            OnRowCountChanging((global::Microsoft.Maui.Controls.Grid)sender, (int)newValue);
            OnRowCountChanging((global::Microsoft.Maui.Controls.Grid)sender, (int)oldValue, (int)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: -1
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetRowCount(global::Microsoft.Maui.Controls.Grid element, int value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(RowCountProperty, value);
        }

        /// <summary>
        /// Default value: -1
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static int GetRowCount(global::Microsoft.Maui.Controls.Grid element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (int)element.GetValue(RowCountProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnRowCountChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnRowCountChanging(global::Microsoft.Maui.Controls.Grid grid);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnRowCountChanging(global::Microsoft.Maui.Controls.Grid grid, int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnRowCountChanging(global::Microsoft.Maui.Controls.Grid grid, int oldValue, int newValue);
    }
}