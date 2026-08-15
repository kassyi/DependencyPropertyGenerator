//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.TypeControlInt.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "TypeControlInt"/> dependency property.<br/>
        /// Default value: default((FrameworkElement, int))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty TypeControlIntProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "TypeControlInt", propertyType: typeof((global::Microsoft.UI.Xaml.FrameworkElement, int)), ownerType: typeof(MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default((global::Microsoft.UI.Xaml.FrameworkElement, int)), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnTypeControlIntChanged();
            ((MyControl)sender).OnTypeControlIntChanged(((global::Microsoft.UI.Xaml.FrameworkElement, int))args.NewValue);
            ((MyControl)sender).OnTypeControlIntChanged(((global::Microsoft.UI.Xaml.FrameworkElement, int))args.OldValue, ((global::Microsoft.UI.Xaml.FrameworkElement, int))args.NewValue);
        }));
        /// <summary>
        /// Default value: default((FrameworkElement, int))
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public (global::Microsoft.UI.Xaml.FrameworkElement, int) TypeControlInt { get => ((global::Microsoft.UI.Xaml.FrameworkElement, int))GetValue(TypeControlIntProperty); set => SetValue(TypeControlIntProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanged((global::Microsoft.UI.Xaml.FrameworkElement, int) newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTypeControlIntChanged((global::Microsoft.UI.Xaml.FrameworkElement, int) oldValue, (global::Microsoft.UI.Xaml.FrameworkElement, int) newValue);
    }
}