//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Test.AttachedProperties.TestProp.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class Test
    {
        /// <summary>
        /// Identifies the TestProp dependency property.<br/>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty TestPropProperty = global::System.Windows.DependencyProperty.RegisterAttached(name: "TestProp", propertyType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test), ownerType: typeof(Test), defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            TestChanged((global::System.Windows.Controls.Grid)sender, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void SetTestProp(global::System.Windows.Controls.Grid element, global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? value)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            element.SetValue(TestPropProperty, value);
        }

        /// <summary>
        /// Default value: default(Test)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [global::System.Windows.AttachedPropertyBrowsableForType(typeof(global::System.Windows.Controls.Grid))]
        public static global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? GetTestProp(global::System.Windows.Controls.Grid element)
        {
            element = element ?? throw new global::System.ArgumentNullException(nameof(element));
            return (global::Kassyi.Generators.DependencyProperty.IntegrationTests.Test? )element.GetValue(TestPropProperty);
        }
    }
}