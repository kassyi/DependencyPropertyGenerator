//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl.Properties.MyProperty2.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class AnotherControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty2"/> dependency property.<br/>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty MyProperty2Property = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "MyProperty2", propertyType: typeof((int, string)), ownerType: typeof(AnotherControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default((int, string)), propertyChangedCallback: static (sender, args) =>
        {
            ((AnotherControl)sender).OnMyProperty2Changed();
            ((AnotherControl)sender).OnMyProperty2Changed(((int, string))args.NewValue);
            ((AnotherControl)sender).OnMyProperty2Changed(((int, string))args.OldValue, ((int, string))args.NewValue);
        }));
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