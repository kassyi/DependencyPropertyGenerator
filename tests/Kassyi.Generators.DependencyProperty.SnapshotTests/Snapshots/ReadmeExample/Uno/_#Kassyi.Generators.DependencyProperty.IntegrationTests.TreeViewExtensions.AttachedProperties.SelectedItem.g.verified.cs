//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.TreeViewExtensions.AttachedProperties.SelectedItem.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public static partial class TreeViewExtensions
{
/// <summary>
/// Identifies the SelectedItem dependency property.<br/>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Windows.UI.Xaml.DependencyProperty SelectedItemProperty =global::Windows.UI.Xaml.DependencyProperty.RegisterAttached(name: "SelectedItem",
propertyType: typeof(object),
ownerType: typeof(TreeViewExtensions),
new global::Windows.UI.Xaml.PropertyMetadata(
    defaultValue: default(object),
    propertyChangedCallback: static (sender, args) =>
{
OnSelectedItemChanged();
OnSelectedItemChanged((global::Windows.UI.Xaml.Controls.TreeView)sender);
OnSelectedItemChanged((global::Windows.UI.Xaml.Controls.TreeView)sender, (object?)args.NewValue);
OnSelectedItemChanged((global::Windows.UI.Xaml.Controls.TreeView)sender, (object?)args.OldValue, (object?)args.NewValue);
}));
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetSelectedItem(global::Windows.UI.Xaml.Controls.TreeView element, object? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(SelectedItemProperty, value);
}
/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static object? GetSelectedItem(global::Windows.UI.Xaml.Controls.TreeView element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (object?)element.GetValue(SelectedItemProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSelectedItemChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSelectedItemChanged(global::Windows.UI.Xaml.Controls.TreeView treeView);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSelectedItemChanged(global::Windows.UI.Xaml.Controls.TreeView treeView, object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnSelectedItemChanged(global::Windows.UI.Xaml.Controls.TreeView treeView, object? oldValue, object? newValue);
}
}
