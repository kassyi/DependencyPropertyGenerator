//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.Properties.Headers.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class Generatable
    {
        /// <summary>
        /// Identifies the <see cref = "Headers"/> dependency property.<br/>
        /// Default value: default(Dictionary&lt;string, string&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.Maui.Controls.BindableProperty HeadersProperty = global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "Headers", returnType: typeof(global::System.Collections.Generic.Dictionary<string, string>), declaringType: typeof(Generatable), defaultValue: default(global::System.Collections.Generic.Dictionary<string, string>), defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.TwoWay, validateValue: null, propertyChanged: static (sender, oldValue, newValue) =>
        {
            ((Generatable)sender).OnHeadersChanged();
            ((Generatable)sender).OnHeadersChanged((global::System.Collections.Generic.Dictionary<string, string>? )newValue);
            ((Generatable)sender).OnHeadersChanged((global::System.Collections.Generic.Dictionary<string, string>? )oldValue, (global::System.Collections.Generic.Dictionary<string, string>? )newValue);
        }, propertyChanging: static (sender, oldValue, newValue) =>
        {
            ((Generatable)sender).OnHeadersChanging();
            ((Generatable)sender).OnHeadersChanging((global::System.Collections.Generic.Dictionary<string, string>? )newValue);
            ((Generatable)sender).OnHeadersChanging((global::System.Collections.Generic.Dictionary<string, string>? )oldValue, (global::System.Collections.Generic.Dictionary<string, string>? )newValue);
        }, coerceValue: null, defaultValueCreator: null);
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
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnHeadersChanging();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnHeadersChanging(global::System.Collections.Generic.Dictionary<string, string>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnHeadersChanging(global::System.Collections.Generic.Dictionary<string, string>? oldValue, global::System.Collections.Generic.Dictionary<string, string>? newValue);
    }
}