//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.GridExtensions.AttachedProperties.UserAgentSuffix.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class GridExtensions
{
/// <summary>
/// Identifies the UserAgentSuffix dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.UI.Xaml.DependencyProperty UserAgentSuffixProperty =global::Microsoft.UI.Xaml.DependencyProperty.RegisterAttached(name: "UserAgentSuffix",
propertyType: typeof(string),
ownerType: typeof(GridExtensions),
new global::Microsoft.UI.Xaml.PropertyMetadata(
    defaultValue: default(string),
    propertyChangedCallback: static (sender, args) =>
{
OnUserAgentSuffixChanged();
OnUserAgentSuffixChanged((global::Microsoft.UI.Xaml.Controls.Grid)sender);
OnUserAgentSuffixChanged((global::Microsoft.UI.Xaml.Controls.Grid)sender, (string?)args.NewValue);
OnUserAgentSuffixChanged((global::Microsoft.UI.Xaml.Controls.Grid)sender, (string?)args.OldValue, (string?)args.NewValue);
}));
/// <summary>
/// A suffix that is added to the default user agent, surrounded by square brackets.
Can be used to identify the web view as belonging to a certain app/version on the server side.<br/>
/// Default value: default(string)
/// </summary>
[global::System.ComponentModel.Description(@"A suffix that is added to the default user agent, surrounded by square brackets.
Can be used to identify the web view as belonging to a certain app/version on the server side.")]
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetUserAgentSuffix(global::Microsoft.UI.Xaml.Controls.Grid element, string? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
if (value is null || value.Length == 0)
{
element.SetValue(UserAgentSuffixProperty, value);
}
else
{
global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetPropertyFromString(element, UserAgentSuffixProperty, value);
}
}
/// <summary>
/// A suffix that is added to the default user agent, surrounded by square brackets.
Can be used to identify the web view as belonging to a certain app/version on the server side.<br/>
/// Default value: default(string)
/// </summary>
[global::System.ComponentModel.Description(@"A suffix that is added to the default user agent, surrounded by square brackets.
Can be used to identify the web view as belonging to a certain app/version on the server side.")]
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static string? GetUserAgentSuffix(global::Microsoft.UI.Xaml.Controls.Grid element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (string?)element.GetValue(UserAgentSuffixProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnUserAgentSuffixChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnUserAgentSuffixChanged(global::Microsoft.UI.Xaml.Controls.Grid grid);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnUserAgentSuffixChanged(global::Microsoft.UI.Xaml.Controls.Grid grid, string? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnUserAgentSuffixChanged(global::Microsoft.UI.Xaml.Controls.Grid grid, string? oldValue, string? newValue);
}
}
