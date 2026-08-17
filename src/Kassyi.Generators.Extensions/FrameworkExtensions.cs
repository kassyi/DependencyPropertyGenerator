using Microsoft.CodeAnalysis.Diagnostics;

namespace Kassyi.Generators.Extensions;

/// <summary>Provides platform-specific naming, resolution, and detection extensions for <see cref="Framework"/>.</summary>
public static class FrameworkExtensions
{
    /// <summary>Error message template displayed when the UI framework cannot be recognized.</summary>
    public const string FrameworkIsNotRecognized = """
        Framework is not recognized.
        You can explicitly specify the framework by setting one of the following constants in your project:
        HAS_WPF, HAS_WINUI, HAS_UWP, HAS_UNO, HAS_UNO_WINUI, HAS_AVALONIA, HAS_MAUI
        """;

    /// <summary>Gets the primary UI root namespace for the target framework.</summary>
    public static string GetNamespace(this Framework framework) => framework switch
    {
        Framework.Wpf => "System.Windows",
        Framework.Uwp or Framework.Uno => "Windows.UI.Xaml",
        Framework.WinUi or Framework.UnoWinUi => "Microsoft.UI.Xaml",
        Framework.Avalonia => "Avalonia",
        Framework.Maui => "Microsoft.Maui.Controls",
        _ => throw new InvalidOperationException($"Platform '{framework}' is not supported."),
    };

    /// <summary>Gets the base object type name (e.g. DependencyObject, BindableObject, AvaloniaObject) for the framework.</summary>
    public static string GetBaseObjectTypeName(this Framework framework) => framework switch
    {
        Framework.Maui => "BindableObject",
        Framework.Avalonia => "AvaloniaObject",
        _ => "DependencyObject",
    };

    /// <summary>Gets the fully qualified base object type name without the global:: prefix.</summary>
    public static string GetBaseObjectTypeFullName(this Framework framework) =>
        $"{framework.GetNamespace()}.{framework.GetBaseObjectTypeName()}";

    /// <summary>Attempts to recognize the target UI framework from MSBuild properties and compilation constants.</summary>
    public static Framework TryRecognizeFramework(this AnalyzerConfigOptionsProvider provider)
    {
        provider = provider ?? throw new ArgumentNullException(nameof(provider));

        var constants = provider.GetGlobalOption("DefineConstants", prefix: "RecognizeFramework") ?? string.Empty;

        if (has("UseMaui", "HAS_MAUI"))
        {
            return Framework.Maui;
        }

        if (has(null, "HAS_AVALONIA"))
        {
            return Framework.Avalonia;
        }

        if (constants.Contains("HAS_UNO_WINUI") || (constants.Contains("HAS_UNO") && constants.Contains("HAS_WINUI")))
        {
            return Framework.UnoWinUi;
        }

        if (has(null, "HAS_UNO"))
        {
            return Framework.Uno;
        }

        if (has("UseWinUI", "HAS_WINUI"))
        {
            return Framework.WinUi;
        }

        if (constants.Contains("WINDOWS_UWP") || constants.Contains("HAS_UWP"))
        {
            return Framework.Uwp;
        }

        return has("UseWPF", "HAS_WPF") ? Framework.Wpf : Framework.None;

        bool has(string? property, string constant) =>
            (property != null && bool.Parse(provider.GetGlobalOption(property) ?? bool.FalseString)) ||
            constants.Contains(constant);
    }

    /// <summary>Recognizes the target UI framework or throws <see cref="InvalidOperationException"/> if unrecognized.</summary>
    public static Framework RecognizeFramework(this AnalyzerConfigOptionsProvider provider)
    {
        var framework = provider.TryRecognizeFramework();
        return framework != Framework.None ? framework 
            : throw new InvalidOperationException(FrameworkIsNotRecognized);
    }
}
