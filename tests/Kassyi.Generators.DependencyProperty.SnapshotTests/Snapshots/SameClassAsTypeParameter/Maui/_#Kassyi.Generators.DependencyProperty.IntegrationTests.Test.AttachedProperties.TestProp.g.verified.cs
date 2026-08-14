//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Test.AttachedProperties.TestProp.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Test
    {
        /// <summary>
        /// Identifies the TestProp dependency property.<br/>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty TestPropProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(propertyName: "TestProp", returnType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test), declaringType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test), defaultValue: default(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            TestChanged((global::Microsoft.Maui.Controls.Grid)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? )newValue);
        }, propertyChanging: null, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetTestProp(global::Microsoft.Maui.Controls.Grid element, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(TestPropProperty, value);
        }

        /// <summary>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? GetTestProp(global::Microsoft.Maui.Controls.Grid element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? )element.GetValue(TestPropProperty);
        }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnTestPropChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnTestPropChanging(global::Microsoft.Maui.Controls.Grid grid);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnTestPropChanging(global::Microsoft.Maui.Controls.Grid grid, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        static partial void OnTestPropChanging(global::Microsoft.Maui.Controls.Grid grid, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? newValue);
    }
}