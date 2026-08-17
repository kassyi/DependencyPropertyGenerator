using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Instantiates the appropriate source generation strategy based on the target UI framework.</summary>
internal static class FrameworkGeneratorFactory
{
    private static readonly WpfFrameworkGenerator s_wpf = new();
    private static readonly AvaloniaFrameworkGenerator s_avalonia = new();
    private static readonly MauiFrameworkGenerator s_maui = new();
    private static readonly UwpFrameworkGenerator s_uwp = new();

    public static FrameworkGenerator Create(Framework framework)
    {
        return framework switch
        {
            Framework.Wpf => s_wpf,
            Framework.Avalonia => s_avalonia,
            Framework.Maui => s_maui,
            Framework.Uwp => s_uwp,
            Framework.WinUi => s_uwp,
            Framework.Uno => s_uwp,
            Framework.UnoWinUi => s_uwp,
            _ => throw new ArgumentOutOfRangeException(nameof(framework))
        };
    }

    public static IDependencyPropertyGeneratorStrategy CreateDependencyPropertyStrategy(Framework framework) => Create(framework);
    public static IRoutedEventGeneratorStrategy CreateRoutedEventStrategy(Framework framework) => Create(framework);
    public static IWeakEventGeneratorStrategy CreateWeakEventStrategy(Framework framework) => Create(framework);
}
