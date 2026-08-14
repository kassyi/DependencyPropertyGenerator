//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.ExplicitUpdateSourceTriggerProperty.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "ExplicitUpdateSourceTriggerProperty"/> dependency property.<br/>
        /// Default value: default(bool)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::Microsoft.UI.Xaml.DependencyProperty ExplicitUpdateSourceTriggerPropertyProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(name: "ExplicitUpdateSourceTriggerProperty", propertyType: typeof(bool), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl), typeMetadata: new global::Microsoft.UI.Xaml.PropertyMetadata(defaultValue: default(bool), propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnExplicitUpdateSourceTriggerPropertyChanged();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnExplicitUpdateSourceTriggerPropertyChanged((bool)args.OldValue, (bool)args.NewValue);
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