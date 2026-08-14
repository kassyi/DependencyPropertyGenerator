//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.Properties.Headers.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Generatable
    {
        /// <summary>
        /// Identifies the <see cref = "Headers"/> dependency property.<br/>
        /// Default value: default(Dictionary&lt;string, string&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty HeadersProperty = global::System.Windows.DependencyProperty.Register(name: "Headers", propertyType: typeof(global::System.Collections.Generic.Dictionary<string, string>), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(global::System.Collections.Generic.Dictionary<string, string>), flags: global::System.Windows.FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable)sender).OnHeadersChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable)sender).OnHeadersChanged((global::System.Collections.Generic.Dictionary<string, string>? )args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable)sender).OnHeadersChanged((global::System.Collections.Generic.Dictionary<string, string>? )args.OldValue, (global::System.Collections.Generic.Dictionary<string, string>? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(Dictionary&lt;string, string&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Collections.Generic.Dictionary<string, string>? Headers { get => (global::System.Collections.Generic.Dictionary<string, string>? )GetValue(HeadersProperty); set => SetValue(HeadersProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnHeadersChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnHeadersChanged(global::System.Collections.Generic.Dictionary<string, string>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnHeadersChanged(global::System.Collections.Generic.Dictionary<string, string>? oldValue, global::System.Collections.Generic.Dictionary<string, string>? newValue);
    }
}