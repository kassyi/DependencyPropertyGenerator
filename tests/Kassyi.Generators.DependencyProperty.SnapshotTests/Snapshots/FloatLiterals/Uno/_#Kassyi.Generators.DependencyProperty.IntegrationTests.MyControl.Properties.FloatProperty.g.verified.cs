//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.FloatProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "FloatProperty"/> dependency property.<br/>
        /// Default value: 42
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Windows.UI.Xaml.DependencyProperty FloatPropertyProperty = global::Windows.UI.Xaml.DependencyProperty.Register(name: "FloatProperty", propertyType: typeof(float), ownerType: typeof(MyControl), typeMetadata: new global::Windows.UI.Xaml.PropertyMetadata(defaultValue: (float)42, propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnFloatPropertyChanged();
            ((MyControl)sender).OnFloatPropertyChanged((float)args.NewValue);
            ((MyControl)sender).OnFloatPropertyChanged((float)args.OldValue, (float)args.NewValue);
        }));
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