//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.Values.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "Values"/> dependency property.<br/>
        /// Default value: default(double[])
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty ValuesProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "Values", propertyType: typeof(double[]), ownerType: typeof(MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default(double[]), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnValuesChanged();
            ((MyControl)sender).OnValuesChanged((double[]? )args.NewValue);
            ((MyControl)sender).OnValuesChanged((double[]? )args.OldValue, (double[]? )args.NewValue);
        }));
        /// <summary>
        /// Default value: default(double[])
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public double[]? Values { get => (double[]? )GetValue(ValuesProperty); set => SetValue(ValuesProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnValuesChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnValuesChanged(double[]? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnValuesChanged(double[]? oldValue, double[]? newValue);
    }
}