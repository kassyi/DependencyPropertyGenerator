//HintName: Namespace1.MyControl.Properties.MyProperty.g.cs
#nullable enable
namespace Namespace1
{
    partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty MyPropertyProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "MyProperty", propertyType: typeof(int), ownerType: typeof(global::Namespace1.MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default(int), propertyChangedCallback: static (sender, args) =>
        {
            ((global::Namespace1.MyControl)sender).OnMyPropertyChanged();
            ((global::Namespace1.MyControl)sender).OnMyPropertyChanged((int)args.NewValue);
            ((global::Namespace1.MyControl)sender).OnMyPropertyChanged((int)args.OldValue, (int)args.NewValue);
        }));
        /// <summary>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public int MyProperty { get => (int)GetValue(MyPropertyProperty); set => SetValue(MyPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(int newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnMyPropertyChanged(int oldValue, int newValue);
    }
}