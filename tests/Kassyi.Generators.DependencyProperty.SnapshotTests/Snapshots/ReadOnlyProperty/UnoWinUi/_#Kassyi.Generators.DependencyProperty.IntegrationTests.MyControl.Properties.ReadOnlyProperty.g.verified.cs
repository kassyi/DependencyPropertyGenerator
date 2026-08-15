//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.ReadOnlyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "ReadOnlyProperty"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty ReadOnlyPropertyProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "ReadOnlyProperty", propertyType: typeof(bool), ownerType: typeof(MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default(bool), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnReadOnlyPropertyChanged();
            ((MyControl)sender).OnReadOnlyPropertyChanged((bool)args.NewValue);
            ((MyControl)sender).OnReadOnlyPropertyChanged((bool)args.OldValue, (bool)args.NewValue);
        }));
        /// <summary>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool ReadOnlyProperty { get => (bool)GetValue(ReadOnlyPropertyProperty); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnReadOnlyPropertyChanged(bool oldValue, bool newValue);
    }
}