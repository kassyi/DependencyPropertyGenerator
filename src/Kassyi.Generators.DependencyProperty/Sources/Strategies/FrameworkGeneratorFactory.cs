using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.DependencyProperty.Sources.Strategies;

/// <summary>Instantiates the appropriate source generation strategy based on the target UI framework.</summary>
internal static class FrameworkGeneratorFactory
{
    public static FrameworkGenerator Create(Framework framework)
    {
        return framework switch
        {
            Framework.Wpf => new WpfFrameworkGenerator(),
            Framework.Avalonia => new AvaloniaFrameworkGenerator(),
            Framework.Maui => new MauiFrameworkGenerator(),
            Framework.Uwp => new UwpFrameworkGenerator(),
            Framework.WinUi => new UwpFrameworkGenerator(),
            Framework.Uno => new UwpFrameworkGenerator(),
            Framework.UnoWinUi => new UwpFrameworkGenerator(),
            _ => throw new ArgumentOutOfRangeException(nameof(framework))
        };
    }

    public static IDependencyPropertyGeneratorStrategy CreateDependencyPropertyStrategy(Framework framework) => Create(framework);
    public static IRoutedEventGeneratorStrategy CreateRoutedEventStrategy(Framework framework) => Create(framework);
    public static IWeakEventGeneratorStrategy CreateWeakEventStrategy(Framework framework) => Create(framework);
}
