//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.ControlA.Properties.MyProperty1.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    sealed public partial class ControlA
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty1"/> dependency property.<br/>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty MyProperty1Property = global::System.Windows.DependencyProperty.Register(name: "MyProperty1", propertyType: typeof(int), ownerType: typeof(ControlA), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(int), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((ControlA)sender).OnMyProperty1Changed();
            ((ControlA)sender).OnMyProperty1Changed((int)args.NewValue);
            ((ControlA)sender).OnMyProperty1Changed((int)args.OldValue, (int)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int MyProperty1 { get => (int)GetValue(MyProperty1Property); set => SetValue(MyProperty1Property, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty1Changed();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty1Changed(int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyProperty1Changed(int oldValue, int newValue);
    }
}