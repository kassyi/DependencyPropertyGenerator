//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.MyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
        /// Default value: default(ReadOnlySpan&lt;byte&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty MyPropertyProperty = global::System.Windows.DependencyProperty.Register(name: "MyProperty", propertyType: typeof(ReadOnlySpan<byte>), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(ReadOnlySpan<byte>), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnMyPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnMyPropertyChanged((ReadOnlySpan<byte>? )args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnMyPropertyChanged((ReadOnlySpan<byte>? )args.OldValue, (ReadOnlySpan<byte>? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(ReadOnlySpan&lt;byte&gt;)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public ReadOnlySpan<byte>? MyProperty { get => (ReadOnlySpan<byte>? )GetValue(MyPropertyProperty); set => SetValue(MyPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(ReadOnlySpan<byte>? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(ReadOnlySpan<byte>? oldValue, ReadOnlySpan<byte>? newValue);
    }
}