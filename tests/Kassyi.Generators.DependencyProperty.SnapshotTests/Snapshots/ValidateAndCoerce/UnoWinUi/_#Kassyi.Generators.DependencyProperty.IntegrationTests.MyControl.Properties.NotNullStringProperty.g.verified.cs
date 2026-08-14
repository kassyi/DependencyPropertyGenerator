//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.NotNullStringProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "NotNullStringProperty"/> dependency property.<br/>
        /// Default value: ""
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty NotNullStringPropertyProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "NotNullStringProperty", propertyType: typeof(string), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: (string)"", propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnNotNullStringPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnNotNullStringPropertyChanged((string)args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnNotNullStringPropertyChanged((string)args.OldValue, (string)args.NewValue);
        }));
        /// <summary>
        /// Default value: ""
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public string NotNullStringProperty { get => (string)GetValue(NotNullStringPropertyProperty); set => SetValue(NotNullStringPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnNotNullStringPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnNotNullStringPropertyChanged(string newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnNotNullStringPropertyChanged(string oldValue, string newValue);
        private partial string CoerceNotNullStringProperty(string? value);
        private static partial bool IsNotNullStringPropertyValid(string? value);
    }
}