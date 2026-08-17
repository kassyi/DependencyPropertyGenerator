//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Properties.Profile.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// Identifies the <see cref="Profile"/> dependency property.<br/>
/// Default value: 0)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty ProfileProperty =
global::System.Windows.DependencyProperty.Register(name: "Profile",
propertyType: typeof(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile),
ownerType: typeof(MyControl),
typeMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)new global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile(1.5, 48.0),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
((MyControl)sender).OnProfileChanged();
((MyControl)sender).OnProfileChanged((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.NewValue);
((MyControl)sender).OnProfileChanged((global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.OldValue, (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);

/// <summary>
/// Default value: 0)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile Profile
{
get => (global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile)GetValue(ProfileProperty);
set => SetValue(ProfileProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnProfileChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnProfileChanged(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnProfileChanged(global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile oldValue, global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyProfile newValue);
}
}
