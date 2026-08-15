//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.Values3.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "Values3"/> dependency property.<br/>
        /// Default value: default(int[,,])
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Windows.UI.Xaml.DependencyProperty Values3Property = global::Windows.UI.Xaml.DependencyProperty.Register(name: "Values3", propertyType: typeof(int[,, ]), ownerType: typeof(MyControl), typeMetadata: new global::Windows.UI.Xaml.PropertyMetadata(defaultValue: default(int[,, ]), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnValues3Changed();
            ((MyControl)sender).OnValues3Changed((int[,, ]? )args.NewValue);
            ((MyControl)sender).OnValues3Changed((int[,, ]? )args.OldValue, (int[,, ]? )args.NewValue);
        }));
        /// <summary>
        /// Default value: default(int[,,])
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int[,, ]? Values3 { get => (int[,, ]? )GetValue(Values3Property); set => SetValue(Values3Property, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnValues3Changed();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnValues3Changed(int[,, ]? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnValues3Changed(int[,, ]? oldValue, int[,, ]? newValue);
    }
}