//HintName: Kassyi.Generators.DependencyProperty.IntegrationTests.MyControl.Events.TrayLeftMouseDown.g.cs

#nullable enable

namespace Kassyi.Generators.DependencyProperty.IntegrationTests
{
public partial class MyControl
{
/// <summary>
/// </summary>
public event global::Windows.UI.Xaml.RoutedEventHandler? TrayLeftMouseDown;

/// <summary>
/// A helper method to raise the TrayLeftMouseDown event.
/// </summary>
protected global::Windows.UI.Xaml.RoutedEventArgs OnTrayLeftMouseDown()
{
var args = new global::Windows.UI.Xaml.RoutedEventArgs();
TrayLeftMouseDown?.Invoke(this, args);

return args;
}
}
}
