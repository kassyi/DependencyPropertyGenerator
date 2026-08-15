//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.Text.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "Text"/> dependency property.<br/>
        /// Default value: default(string)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty TextProperty = global::System.Windows.DependencyProperty.Register(name: "Text", propertyType: typeof(string), ownerType: typeof(MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(string), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnTextChanged();
            ((MyControl)sender).OnTextChanged((string? )args.NewValue);
            ((MyControl)sender).OnTextChanged((string? )args.OldValue, (string? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(string)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public string? Text { get => (string? )GetValue(TextProperty); set => SetValue(TextProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTextChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTextChanged(string? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnTextChanged(string? oldValue, string? newValue);
    }
}