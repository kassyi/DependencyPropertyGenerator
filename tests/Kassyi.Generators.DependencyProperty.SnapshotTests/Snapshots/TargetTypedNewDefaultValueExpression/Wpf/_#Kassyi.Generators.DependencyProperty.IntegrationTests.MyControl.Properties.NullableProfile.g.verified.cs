//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.NullableProfile.g.cs
#nullable enable
namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
    public partial class MyControl
    {
        /// <summary>
        /// Identifies the <see cref = "NullableProfile"/> dependency property.<br/>
        /// Default value: 0)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        public static readonly global::System.Windows.DependencyProperty NullableProfileProperty = global::System.Windows.DependencyProperty.Register(name: "NullableProfile", propertyType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? ), ownerType: typeof(MyControl), typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(defaultValue: (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? )new global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile(1.5, 48.0), flags: global::System.Windows.FrameworkPropertyMetadataOptions.None, propertyChangedCallback: static (sender, args) =>
        {
            ((MyControl)sender).OnNullableProfileChanged();
            ((MyControl)sender).OnNullableProfileChanged((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? )args.NewValue);
            ((MyControl)sender).OnNullableProfileChanged((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? )args.OldValue, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? )args.NewValue);
        }, coerceValueCallback: null, isAnimationProhibited: false), validateValueCallback: null);
        /// <summary>
        /// Default value: 0)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? NullableProfile { get => (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? )GetValue(NullableProfileProperty); set => SetValue(NullableProfileProperty, value); }

        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnNullableProfileChanged();
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnNullableProfileChanged(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? newValue);
        [global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
        partial void OnNullableProfileChanged(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile? newValue);
    }
}