//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.MyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
        /// Default value: default(List&lt;int?&gt;?[])
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty MyPropertyProperty = global::System.Windows.DependencyProperty.Register(name: "MyProperty", propertyType: typeof(global::System.Collections.Generic.List<int?>[]), ownerType: typeof(MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: default(global::System.Collections.Generic.List<int?>[]), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnMyPropertyChanged();
            ((MyControl)sender).OnMyPropertyChanged((global::System.Collections.Generic.List<int?>[]? )args.NewValue);
            ((MyControl)sender).OnMyPropertyChanged((global::System.Collections.Generic.List<int?>[]? )args.OldValue, (global::System.Collections.Generic.List<int?>[]? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: default(List&lt;int?&gt;?[])
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Collections.Generic.List<int?>[]? MyProperty { get => (global::System.Collections.Generic.List<int?>[]? )GetValue(MyPropertyProperty); set => SetValue(MyPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(global::System.Collections.Generic.List<int?>[]? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(global::System.Collections.Generic.List<int?>[]? oldValue, global::System.Collections.Generic.List<int?>[]? newValue);
    }
}