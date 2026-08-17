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
        public static readonly global::Microsoft.Maui.Controls.BindableProperty IsSpinningProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "IsSpinning", returnType: typeof(bool), declaringType: typeof(MyControl), defaultValue: default(bool), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((MyControl)sender).OnIsSpinningChanged();
            ((MyControl)sender).OnIsSpinningChanged((bool)newValue);
            ((MyControl)sender).OnIsSpinningChanged((bool)oldValue, (bool)newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((MyControl)sender).OnIsSpinningChanging();
            ((MyControl)sender).OnIsSpinningChanging((bool)newValue);
            ((MyControl)sender).OnIsSpinningChanging((bool)oldValue, (bool)newValue);
        }, coerceValue: null, defaultValueCreator: null);
        /// <summary>
        /// &lt;see cref=&quot;Style.TargetType&quot;/&gt; must be Label.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.ComponentModel.Description("&lt;see cref=&quot;Style.TargetType&quot;/&gt; must be Label.")]
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool IsSpinning { get => (bool)GetValue(IsSpinningProperty); set => SetValue(IsSpinningProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanged(bool oldValue, bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanging(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnIsSpinningChanging(bool oldValue, bool newValue);
    }
}