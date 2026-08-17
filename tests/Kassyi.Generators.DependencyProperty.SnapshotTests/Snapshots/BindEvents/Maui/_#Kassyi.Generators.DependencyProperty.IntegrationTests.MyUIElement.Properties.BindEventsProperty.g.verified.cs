//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyUIElement.Properties.BindEventsProperty.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyUIElement
{
/// <summary>
/// Identifies the <see cref="BindEventsProperty"/> dependency property.<br/>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
public static readonly global::Microsoft.Maui.Controls.BindableProperty BindEventsPropertyProperty =
global::Microsoft.Maui.Controls.BindableProperty.Create(propertyName: "BindEventsProperty",
returnType: typeof(object),
declaringType: typeof(MyUIElement),
defaultValue: default(object),
defaultBindingMode: global::Microsoft.Maui.Controls.BindingMode.OneWay,
validateValue: null,
propertyChanged: static (sender, oldValue, newValue) =>
{
((MyUIElement)sender).OnBindEventsPropertyChanged();
((MyUIElement)sender).OnBindEventsPropertyChanged((object?)newValue);
((MyUIElement)sender).OnBindEventsPropertyChanged((object?)oldValue, (object?)newValue);
},
propertyChanging: static (sender, oldValue, newValue) =>
{
((MyUIElement)sender).OnBindEventsPropertyChanging();
((MyUIElement)sender).OnBindEventsPropertyChanging((object?)newValue);
((MyUIElement)sender).OnBindEventsPropertyChanging((object?)oldValue, (object?)newValue);
},
coerceValue: null,
defaultValueCreator: null);

/// <summary>
/// Default value: default(object)
/// </summary>
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public object? BindEventsProperty
{
get => (object?)GetValue(BindEventsPropertyProperty);
set => SetValue(BindEventsPropertyProperty, value);

}

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnBindEventsPropertyChanged();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnBindEventsPropertyChanged(object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnBindEventsPropertyChanged(object? oldValue, object? newValue);

[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnBindEventsPropertyChanging();
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnBindEventsPropertyChanging(object? newValue);
[global::System.CodeDom.Compiler.GeneratedCode("DependencyPropertyGenerator", "0.0.0.0")]
partial void OnBindEventsPropertyChanging(object? oldValue, object? newValue);

partial void OnBindEventsPropertyChanged_BeforeBind(object? oldValue, object? newValue);
partial void OnBindEventsPropertyChanged_AfterBind(object? oldValue, object? newValue);

partial void OnBindEventsPropertyChanged(object? oldValue, object? newValue)
{
OnBindEventsPropertyChanged_BeforeBind(oldValue, newValue);

if (oldValue is not default(object))
{
this.Loaded -= OnBindEventsPropertyChanged_Loaded;
this.Unloaded -= OnBindEventsPropertyChanged_Unloaded;
}
if (newValue is not default(object))
{
this.Loaded += OnBindEventsPropertyChanged_Loaded;
this.Unloaded += OnBindEventsPropertyChanged_Unloaded;
}

OnBindEventsPropertyChanged_AfterBind(oldValue, newValue);
}
}
}
