//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.TupleControlInt.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "TupleControlInt"/> dependency property.<br/>
        /// Default value: default(Tuple&lt;FrameworkElement, int&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty TupleControlIntProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "TupleControlInt", propertyType: typeof(global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>), ownerType: typeof(MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default(global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnTupleControlIntChanged();
            ((MyControl)sender).OnTupleControlIntChanged((global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? )args.NewValue);
            ((MyControl)sender).OnTupleControlIntChanged((global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? )args.OldValue, (global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? )args.NewValue);
        }));
        /// <summary>
        /// Default value: default(Tuple&lt;FrameworkElement, int&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? TupleControlInt { get => (global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? )GetValue(TupleControlIntProperty); set => SetValue(TupleControlIntProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleControlIntChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleControlIntChanged(global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleControlIntChanged(global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? oldValue, global::System.Tuple<global::Microsoft.UI.Xaml.FrameworkElement, int>? newValue);
    }
}