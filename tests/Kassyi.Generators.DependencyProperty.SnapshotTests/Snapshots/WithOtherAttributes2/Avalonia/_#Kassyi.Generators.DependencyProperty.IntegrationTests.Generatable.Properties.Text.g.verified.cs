//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable.Properties.Text.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class Generatable
    {
        /// <summary>
        /// Identifies the <see cref = "Text"/> dependency property.<br/>
        /// Default value: default(string)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Avalonia.StyledProperty<string?> TextProperty = global::Avalonia.AvaloniaProperty.Register<global::Kassyi.Generators.DependencyProperty.IntegrationTests.Generatable, string?>(name: "Text", defaultValue: default(string), inherits: false, defaultBindingMode: global::Avalonia.Data.BindingMode.OneWay, validate: null, coerce: null);
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