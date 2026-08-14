//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.FloatProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "FloatProperty"/> dependency property.<br/>
        /// Default value: 42
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty FloatPropertyProperty = global::System.Windows.DependencyProperty.Register(name: "FloatProperty", propertyType: typeof(float), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: (float)42, flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnFloatPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnFloatPropertyChanged((float)args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnFloatPropertyChanged((float)args.OldValue, (float)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: 42
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public float FloatProperty { get => (float)GetValue(FloatPropertyProperty); set => SetValue(FloatPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanged(float newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnFloatPropertyChanged(float oldValue, float newValue);
    }
}