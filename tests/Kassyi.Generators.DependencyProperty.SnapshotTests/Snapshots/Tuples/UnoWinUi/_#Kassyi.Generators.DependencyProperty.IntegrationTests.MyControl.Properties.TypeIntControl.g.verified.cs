//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.TypeIntControl.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "TypeIntControl"/> dependency property.<br/>
        /// Default value: default((int, FrameworkElement))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty TypeIntControlProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "TypeIntControl", propertyType: typeof((int, global::Microsoft.UI.Xaml.FrameworkElement)), ownerType: typeof(MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default((int, global::Microsoft.UI.Xaml.FrameworkElement)), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnTypeIntControlChanged();
            ((MyControl)sender).OnTypeIntControlChanged(((int, global::Microsoft.UI.Xaml.FrameworkElement))args.NewValue);
            ((MyControl)sender).OnTypeIntControlChanged(((int, global::Microsoft.UI.Xaml.FrameworkElement))args.OldValue, ((int, global::Microsoft.UI.Xaml.FrameworkElement))args.NewValue);
        }));
        /// <summary>
        /// Default value: default((int, FrameworkElement))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public (int, global::Microsoft.UI.Xaml.FrameworkElement) TypeIntControl { get => ((int, global::Microsoft.UI.Xaml.FrameworkElement))GetValue(TypeIntControlProperty); set => SetValue(TypeIntControlProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntControlChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntControlChanged((int, global::Microsoft.UI.Xaml.FrameworkElement) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeIntControlChanged((int, global::Microsoft.UI.Xaml.FrameworkElement) oldValue, (int, global::Microsoft.UI.Xaml.FrameworkElement) newValue);
    }
}