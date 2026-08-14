//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.TypeIntString.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "TypeIntString"/> dependency property.<br/>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty TypeIntStringProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "TypeIntString", propertyType: typeof((int, string)), ownerType: typeof(MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default((int, string)), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnTypeIntStringChanged();
            ((MyControl)sender).OnTypeIntStringChanged(((int, string))args.NewValue);
            ((MyControl)sender).OnTypeIntStringChanged(((int, string))args.OldValue, ((int, string))args.NewValue);
        }));
        /// <summary>
        /// Default value: default((int, string))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public (int, string) TypeIntString { get => ((int, string))GetValue(TypeIntStringProperty); set => SetValue(TypeIntStringProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanged((int, string) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntStringChanged((int, string) oldValue, (int, string) newValue);
    }
}