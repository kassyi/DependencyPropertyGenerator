//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.TupleIntControl.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "TupleIntControl"/> dependency property.<br/>
        /// Default value: default(Tuple&lt;int, FrameworkElement&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Windows.UI.Xaml.DependencyProperty TupleIntControlProperty = global::Windows.UI.Xaml.DependencyProperty.Register(name: "TupleIntControl", propertyType: typeof(global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>), ownerType: typeof(MyControl), typeMetadata: new global::Windows.UI.Xaml.PropertyMetadata(defaultValue: default(global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnTupleIntControlChanged();
            ((MyControl)sender).OnTupleIntControlChanged((global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? )args.NewValue);
            ((MyControl)sender).OnTupleIntControlChanged((global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? )args.OldValue, (global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? )args.NewValue);
        }));
        /// <summary>
        /// Default value: default(Tuple&lt;int, FrameworkElement&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? TupleIntControl { get => (global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? )GetValue(TupleIntControlProperty); set => SetValue(TupleIntControlProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanged(global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTupleIntControlChanged(global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? oldValue, global::System.Tuple<int, global::Windows.UI.Xaml.FrameworkElement>? newValue);
    }
}