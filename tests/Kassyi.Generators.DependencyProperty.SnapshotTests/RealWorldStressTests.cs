#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class RealWorldStressTests
{
    public static IEnumerable<object[]> GetStressTestTargets()
    {
        var targets = new List<StressTestTarget>
        {
            // 1. Wpf.Ui
            new("WpfUi", "https://github.com/lepoco/wpfui/archive/ffebacd61058170cf63864b7d5aa730cffff848a.zip", "wpfui-ffebacd61058170cf63864b7d5aa730cffff848a", "src/Wpf.Ui", PlatformType.Wpf),
            // 2. MaterialDesignInXamlToolkit
            new("MaterialDesign", "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/archive/refs/heads/master.zip", "MaterialDesignInXamlToolkit-master", "src/MaterialDesignThemes.Wpf", PlatformType.Wpf),
            // 3. MahApps.Metro
            new("MahApps.Metro", "https://github.com/MahApps/MahApps.Metro/archive/refs/heads/main.zip", "MahApps.Metro-main", "src/MahApps.Metro", PlatformType.Wpf),
            // 4. HandyControl
            new("HandyControl", "https://github.com/HandyOrg/HandyControl/archive/refs/heads/master.zip", "HandyControl-master", "src/Shared", PlatformType.Wpf),
            // 5. ControlzEx
            new("ControlzEx", "https://github.com/ControlzEx/ControlzEx/archive/refs/heads/develop.zip", "ControlzEx-develop", "src/ControlzEx", PlatformType.Wpf),
            // 6. Fluent.Ribbon
            new("Fluent.Ribbon", "https://github.com/fluentribbon/Fluent.Ribbon/archive/refs/heads/master.zip", "Fluent.Ribbon-master", "Fluent.Ribbon", PlatformType.Wpf),
            // 7. ModernWpf
            new("ModernWpf", "https://github.com/Kinnara/ModernWpf/archive/refs/heads/master.zip", "ModernWpf-master", "ModernWpf/Controls", PlatformType.Wpf),
            // 8. Dragablz
            new("Dragablz", "https://github.com/ButchersBoy/Dragablz/archive/refs/heads/master.zip", "Dragablz-master", "Dragablz", PlatformType.Wpf),
            // 9. GongSolutions.WPF.DragDrop
            new("GongDragDrop", "https://github.com/punker76/gong-wpf-dragdrop/archive/refs/heads/main.zip", "gong-wpf-dragdrop-main", "src/GongSolutions.WPF.DragDrop", PlatformType.Wpf),
            // 10. Nodify
            new("Nodify", "https://github.com/miroiu/nodify/archive/refs/heads/master.zip", "nodify-master", "Nodify", PlatformType.Wpf),
            // 11. Panuon.WPF.UI
            new("Panuon.WPF.UI", "https://github.com/Panuon/Panuon.WPF.UI/archive/refs/heads/master.zip", "Panuon.WPF.UI-master", "SourceCode", PlatformType.Wpf),
            // 12. LiveCharts2
            new("LiveCharts2", "https://github.com/beto-rodriguez/LiveCharts2/archive/refs/heads/master.zip", "LiveCharts2-master", "src", PlatformType.Wpf),
            // 13. OxyPlot
            new("OxyPlot", "https://github.com/oxyplot/oxyplot/archive/refs/heads/master.zip", "oxyplot-master", "Source", PlatformType.Wpf),
            // 14. Avalonia
            new("Avalonia", "https://github.com/AvaloniaUI/Avalonia/archive/refs/heads/master.zip", "Avalonia-main", "src", PlatformType.Avalonia),
            // 15. AvaloniaEdit
            new("AvaloniaEdit", "https://github.com/AvaloniaUI/AvaloniaEdit/archive/refs/heads/master.zip", "AvaloniaEdit-master", "src", PlatformType.Avalonia),
            // 16. microsoft-ui-xaml (WinUI)
            new("WinUI", "https://github.com/microsoft/microsoft-ui-xaml/archive/refs/heads/main.zip", "microsoft-ui-xaml-main", "controls", PlatformType.WinUI),
            // 17. WindowsCommunityToolkit (WinUI/UWP)
            new("WindowsCommunityToolkit", "https://github.com/CommunityToolkit/WindowsCommunityToolkit/archive/refs/heads/main.zip", "WindowsCommunityToolkit-main", ".", PlatformType.WinUI),
            // 18. dotnet/maui
            new("MAUI", "https://github.com/dotnet/maui/archive/refs/heads/main.zip", "maui-main", "src/Controls", PlatformType.Maui),
            // 19. CommunityToolkit.Maui
            new("MauiCommunityToolkit", "https://github.com/CommunityToolkit/Maui/archive/refs/heads/main.zip", "Maui-main", "src", PlatformType.Maui),
            // 20. Uno
            new("Uno", "https://github.com/unoplatform/uno/archive/refs/heads/master.zip", "uno-master", "src/Uno.UI", PlatformType.Uno)
        };

        bool isCi = Environment.GetEnvironmentVariable("CI") == "true" || Environment.GetEnvironmentVariable("CI") == "1";
        
        // CI環境では実行時間とレートリミットを考慮して上位3つ(WpfUi, MaterialDesign, MahApps)のみに絞る
        if (isCi)
        {
            targets = targets.Take(3).ToList();
        }

        foreach (var target in targets)
        {
            yield return new object[] { target };
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(GetStressTestTargets), DynamicDataSourceType.Method)]
    [TestCategory("Stress")]
    public async Task RunStressTest(StressTestTarget target)
    {
        await StressTestRunner.RunAsync(target);
    }
}
