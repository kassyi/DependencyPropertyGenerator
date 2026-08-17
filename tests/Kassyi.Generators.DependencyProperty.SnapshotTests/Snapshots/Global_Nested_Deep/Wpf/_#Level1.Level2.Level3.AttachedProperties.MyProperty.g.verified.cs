//HintName: Level1.Level2.Level3.AttachedProperties.MyProperty.g.cs

#nullable enable

public partial class Level1
{
public partial class Level2
{
public partial record Level3
{
/// <summary>
/// Identifies the MyProperty dependency property.<br/>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::System.Windows.DependencyProperty MyPropertyProperty =global::System.Windows.DependencyProperty.RegisterAttached(name: "MyProperty",
propertyType: typeof(string),
ownerType: typeof(Level3),
defaultMetadata: new global::System.Windows.FrameworkPropertyMetadata(
    defaultValue: default(string),
    flags: global::System.Windows.FrameworkPropertyMetadataOptions.None,
    propertyChangedCallback: static (sender, args) =>
{
OnMyPropertyChanged();
OnMyPropertyChanged((global::System.Windows.DependencyObject)sender);
OnMyPropertyChanged((global::System.Windows.DependencyObject)sender, (string?)args.NewValue);
OnMyPropertyChanged((global::System.Windows.DependencyObject)sender, (string?)args.OldValue, (string?)args.NewValue);
},
    coerceValueCallback: null,
    isAnimationProhibited: false),
validateValueCallback: null);
/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static void SetMyProperty(global::System.Windows.DependencyObject element, string? value)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
element.SetValue(MyPropertyProperty, value);
}
/// <summary>
/// Default value: default(string)
/// </summary>
[global::System.Windows.AttachedPropertyBrowsableForType(typeof(global::System.Windows.DependencyObject))]
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static string? GetMyProperty(global::System.Windows.DependencyObject element)
{
element = element ?? throw new global::System.ArgumentNullException(nameof(element));
return (string?)element.GetValue(MyPropertyProperty);
}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged(global::System.Windows.DependencyObject dependencyObject);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged(global::System.Windows.DependencyObject dependencyObject, string? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
static partial void OnMyPropertyChanged(global::System.Windows.DependencyObject dependencyObject, string? oldValue, string? newValue);
}
}
}
