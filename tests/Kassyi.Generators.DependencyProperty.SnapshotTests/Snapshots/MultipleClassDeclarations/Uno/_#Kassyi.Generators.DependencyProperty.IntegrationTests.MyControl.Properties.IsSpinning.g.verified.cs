//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.IsSpinning.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "IsSpinning"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Windows.UI.Xaml.DependencyProperty IsSpinningProperty = global::Windows.UI.Xaml.DependencyProperty.Register(name: "IsSpinning", propertyType: typeof(bool), ownerType: typeof(MyControl), typeMetadata: new global::Windows.UI.Xaml.PropertyMetadata(defaultValue: default(bool), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnIsSpinningChanged();
            ((MyControl)sender).OnIsSpinningChanged((bool)args.NewValue);
            ((MyControl)sender).OnIsSpinningChanged((bool)args.OldValue, (bool)args.NewValue);
        }));
        /// <summary>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool IsSpinning { get => (bool)GetValue(IsSpinningProperty); set => SetValue(IsSpinningProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool oldValue, bool newValue);
    }
}