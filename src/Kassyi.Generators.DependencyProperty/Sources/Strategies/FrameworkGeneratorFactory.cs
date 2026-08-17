using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Instantiates the appropriate source generation strategy based on the target UI framework.</summary>
internal static class FrameworkGeneratorFactory
{
    private static readonly WpfFrameworkGenerator s_wpf = new();
    private static readonly AvaloniaFrameworkGenerator s_avalonia = new();
    private static readonly MauiFrameworkGenerator s_maui = new();
    private static readonly UwpFrameworkGenerator s_uwp = new();

    public static FrameworkGenerator Create(Framework framework) => framework switch
    {
        Framework.Wpf => s_wpf,
        Framework.Avalonia => s_avalonia,
        Framework.Maui => s_maui,
        Framework.Uwp or Framework.WinUi or Framework.Uno or Framework.UnoWinUi => s_uwp,
        _ => throw new ArgumentOutOfRangeException(nameof(framework), framework, null)
    };

    public static IDependencyPropertyGeneratorStrategy CreateDependencyPropertyStrategy(Framework framework) => Create(framework);
    public static IRoutedEventGeneratorStrategy CreateRoutedEventStrategy(Framework framework) => Create(framework);
    public static IWeakEventGeneratorStrategy CreateWeakEventStrategy(Framework framework) => Create(framework);
}
