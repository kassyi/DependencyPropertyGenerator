//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl.Properties.MyProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class AnotherControl
    {
        /// <summary>
        /// Identifies the <see cref = "MyProperty"/> dependency property.<br/>
        /// Default value: default(int)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Windows.UI.Xaml.DependencyProperty MyPropertyProperty = global::Windows.UI.Xaml.DependencyProperty.Register(name: "MyProperty", propertyType: typeof(int), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl), typeMetadata: new global::Windows.UI.Xaml.PropertyMetadata(defaultValue: default(int), propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanged((int)args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.AnotherControl)sender).OnMyPropertyChanged((int)args.OldValue, (int)args.NewValue);
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