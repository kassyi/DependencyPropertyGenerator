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
        public static readonly global::Avalonia.AttachedProperty<global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test?> TestPropProperty = global::Avalonia.AvaloniaProperty.RegisterAttached<global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test, global::Avalonia.Controls.Grid, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test?>(name: "TestProp", defaultValue: default(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test), inherits: false, defaultBindingMode: global::Avalonia.Data.BindingMode.OneWay, validate: null, coerce: null);
        /// <summary>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetTestProp(global::Avalonia.Controls.Grid element, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(TestPropProperty, value);
        }

        /// <summary>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? GetTestProp(global::Avalonia.Controls.Grid element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? )element.GetValue(TestPropProperty);
        }
    }
}