//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.BrokenProfile1.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "BrokenProfile1"/> dependency property.<br/>
        /// Default value: 0
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty BrokenProfile1Property = global::System.Windows.DependencyProperty.Register(name: "BrokenProfile1", propertyType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile), ownerType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)new global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile(1.5, 48.0, flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnBrokenProfile1Changed();
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnBrokenProfile1Changed((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.NewValue);
            ((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl)sender).OnBrokenProfile1Changed((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.OldValue, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: 0
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile BrokenProfile1 { get => (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)GetValue(BrokenProfile1Property); set => SetValue(BrokenProfile1Property, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnBrokenProfile1Changed();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnBrokenProfile1Changed(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnBrokenProfile1Changed(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile newValue);
    }
}