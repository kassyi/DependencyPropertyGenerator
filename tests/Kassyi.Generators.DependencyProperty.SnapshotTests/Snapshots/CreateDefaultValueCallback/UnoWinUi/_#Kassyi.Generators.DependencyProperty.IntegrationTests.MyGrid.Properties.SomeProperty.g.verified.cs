//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyGrid.Properties.SomeProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyGrid
    {
        /// <summary>
        /// Identifies the <see cref = "SomeProperty"/> dependency property.<br/>
        /// Default value: default(string)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty SomePropertyProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "SomeProperty", propertyType: typeof(string), ownerType: typeof(MyGrid), typeMetadata: global::Microsoft.UI.Xaml.PropertyMetadata.Create(createDefaultValueCallback: static () => GetSomePropertyDefaultValue(), propertyChangedCallback: static (sender, args) =>
        {
            ((MyGrid)sender).OnSomePropertyChanged();
            ((MyGrid)sender).OnSomePropertyChanged((string? )args.NewValue);
            ((MyGrid)sender).OnSomePropertyChanged((string? )args.OldValue, (string? )args.NewValue);
        }));
        /// <summary>
        /// Default value: default(string)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public string? SomeProperty { get => (string? )GetValue(SomePropertyProperty); set => SetValue(SomePropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnSomePropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnSomePropertyChanged(string? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnSomePropertyChanged(string? oldValue, string? newValue);
        private static partial string? GetSomePropertyDefaultValue();
    }
}