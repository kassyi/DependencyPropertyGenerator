//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl.Properties.MyProperty2.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class AnotherControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty2"/> dependency property.<br/>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty MyProperty2Property = global::System.Windows.DependencyProperty.Register(name: "MyProperty2", propertyType: typeof((int, string)), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default((int, string)), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyProperty2Changed();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyProperty2Changed(((int, string))args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyProperty2Changed(((int, string))args.OldValue, ((int, string))args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public (int, string) MyProperty2 { get => ((int, string))GetValue(MyProperty2Property); set => SetValue(MyProperty2Property, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed((int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed((int, string) oldValue, (int, string) newValue);
    }
}