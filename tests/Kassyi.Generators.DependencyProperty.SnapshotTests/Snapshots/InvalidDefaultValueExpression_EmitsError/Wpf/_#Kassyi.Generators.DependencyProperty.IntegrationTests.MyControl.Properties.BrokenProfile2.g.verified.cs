//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.BrokenProfile2.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "BrokenProfile2"/> dependency property.<br/>
        /// Default value: MyProfile(???
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty BrokenProfile2Property = global::System.Windows.DependencyProperty.Register(name: "BrokenProfile2", propertyType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile), ownerType: typeof(MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)new global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile( ?? ? , flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnBrokenProfile2Changed();
            ((MyControl)sender).OnBrokenProfile2Changed((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.NewValue);
            ((MyControl)sender).OnBrokenProfile2Changed((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.OldValue, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: MyProfile(???
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile BrokenProfile2 { get => (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)GetValue(BrokenProfile2Property); set => SetValue(BrokenProfile2Property, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnBrokenProfile2Changed();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnBrokenProfile2Changed(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnBrokenProfile2Changed(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile newValue);
    }
}