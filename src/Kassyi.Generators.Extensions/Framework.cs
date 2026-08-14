namespace Kassyi.Generators.Extensions;

/// <summary>Specifies the target UI framework for platform-specific source generation.</summary>
public enum Framework
{
    /// <summary>Unrecognized UI framework.</summary>
    None,

    /// <summary>Windows Presentation Foundation (WPF).</summary>
    Wpf,

    /// <summary>Universal Windows Platform (UWP).</summary>
    Uwp,

    /// <summary>WinUI 3 / Windows App SDK.</summary>
    WinUi,

    /// <summary>Uno Platform (UWP).</summary>
    Uno,

    /// <summary>Uno Platform (WinUI).</summary>
    UnoWinUi,

    /// <summary>Avalonia UI.</summary>
    Avalonia,

    /// <summary>.NET MAUI.</summary>
    Maui,
}
