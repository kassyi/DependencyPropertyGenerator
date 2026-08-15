//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.ControlB.Properties.MyProperty2.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public sealed partial class ControlB
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty2"/> dependency property.<br/>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty MyProperty2Property = global::System.Windows.DependencyProperty.Register(name: "MyProperty2", propertyType: typeof(int), ownerType: typeof(ControlB), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(int), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((ControlB)sender).OnMyProperty2Changed();
            ((ControlB)sender).OnMyProperty2Changed((int)args.NewValue);
            ((ControlB)sender).OnMyProperty2Changed((int)args.OldValue, (int)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int MyProperty2 { get => (int)GetValue(MyProperty2Property); set => SetValue(MyProperty2Property, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed(int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty2Changed(int oldValue, int newValue);
    }
}