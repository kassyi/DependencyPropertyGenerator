using Microsoft.CodeAnalysis;
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

    /// <summary>Attempts to recognize the target UI framework by inspecting referenced type symbols in the compilation.</summary>
    public static Framework TryRecognizeFramework(this Compilation compilation)
    {
        compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));

        return ResolveFramework(
            isMaui: () => compilation.GetTypeByMetadataName("Microsoft.Maui.Controls.BindableObject") != null,
            isAvalonia: () => compilation.GetTypeByMetadataName("Avalonia.AvaloniaObject") != null,
            hasUno: () => compilation.GetTypeByMetadataName("Uno.UI.FeatureConfiguration") != null,
            hasWinUi: () => compilation.GetTypeByMetadataName("Microsoft.UI.Xaml.DependencyObject") != null,
            isUwp: () => compilation.GetTypeByMetadataName("Windows.UI.Xaml.DependencyObject") != null,
            isWpf: () => compilation.GetTypeByMetadataName("System.Windows.DependencyObject") != null);
    }

    /// <summary>Attempts to recognize the target UI framework from MSBuild properties and compilation constants.</summary>
    public static Framework TryRecognizeFramework(this AnalyzerConfigOptionsProvider provider)
    {
        provider = provider ?? throw new ArgumentNullException(nameof(provider));

        var constants = provider.GetGlobalOption("DefineConstants", prefix: "RecognizeFramework") ?? string.Empty;

        return ResolveFramework(
            isMaui: () => has("UseMaui", "HAS_MAUI"),
            isAvalonia: () => has(null, "HAS_AVALONIA"),
            hasUno: () => has(null, "HAS_UNO") || hasConstant("HAS_UNO_WINUI"),
            hasWinUi: () => has("UseWinUI", "HAS_WINUI") || hasConstant("HAS_UNO_WINUI"),
            isUwp: () => hasConstant("WINDOWS_UWP") || hasConstant("HAS_UWP"),
            isWpf: () => has("UseWPF", "HAS_WPF"));

        bool has(string? property, string constant) =>
            (property != null && bool.TryParse(provider.GetGlobalOption(property), out var enabled) && enabled) ||
            hasConstant(constant);

        bool hasConstant(string constant)
        {
            foreach (var part in constants.Split(';'))
            {
                if (string.Equals(part.Trim(), constant, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private static Framework ResolveFramework(
        Func<bool> isMaui,
        Func<bool> isAvalonia,
        Func<bool> hasUno,
        Func<bool> hasWinUi,
        Func<bool> isUwp,
        Func<bool> isWpf)
    {
        if (isMaui())
        {
            return Framework.Maui;
        }

        if (isAvalonia())
        {
            return Framework.Avalonia;
        }

        var uno = hasUno();
        var winUi = hasWinUi();

        if (uno && winUi)
        {
            return Framework.UnoWinUi;
        }

        if (uno)
        {
            return Framework.Uno;
        }

        if (winUi)
        {
            return Framework.WinUi;
        }

        if (isUwp())
        {
            return Framework.Uwp;
        }

        if (isWpf())
        {
            return Framework.Wpf;
        }

        return Framework.None;
    }

    /// <summary>Attempts to recognize the target UI framework from compilation symbols, falling back to build configuration options.</summary>
    public static Framework TryRecognizeFramework(this Compilation compilation, AnalyzerConfigOptionsProvider? options)
    {
        var framework = compilation.TryRecognizeFramework();
        if (framework != Framework.None)
        {
            return framework;
        }

        return options?.TryRecognizeFramework() ?? Framework.None;
    }

    /// <summary>Recognizes the target UI framework or throws <see cref="InvalidOperationException"/> if unrecognized.</summary>
    public static Framework RecognizeFramework(this AnalyzerConfigOptionsProvider provider)
    {
        var framework = provider.TryRecognizeFramework();
        return framework != Framework.None ? framework 
            : throw new InvalidOperationException(FrameworkIsNotRecognized);
    }
}
