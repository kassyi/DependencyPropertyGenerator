//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.ExplicitUpdateSourceTriggerProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "ExplicitUpdateSourceTriggerProperty"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Windows.UI.Xaml.DependencyProperty ExplicitUpdateSourceTriggerPropertyProperty = global::Windows.UI.Xaml.DependencyProperty.Register(name: "ExplicitUpdateSourceTriggerProperty", propertyType: typeof(bool), ownerType: typeof(MyControl), typeMetadata: new global::Windows.UI.Xaml.PropertyMetadata(defaultValue: default(bool), propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnExplicitUpdateSourceTriggerPropertyChanged();
            ((MyControl)sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)args.NewValue);
            ((MyControl)sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)args.OldValue, (bool)args.NewValue);
        }));
        /// <summary>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool ExplicitUpdateSourceTriggerProperty { get => (bool)GetValue(ExplicitUpdateSourceTriggerPropertyProperty); set => SetValue(ExplicitUpdateSourceTriggerPropertyProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanged(bool newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnExplicitUpdateSourceTriggerPropertyChanged(bool oldValue, bool newValue);
    }
}