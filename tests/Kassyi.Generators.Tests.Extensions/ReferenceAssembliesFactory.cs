using System.Collections.Concurrent;
using System.Collections.Immutable;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis.Testing;

namespace Kassyi.Generators.Tests.Extensions;

public static class ReferenceAssembliesFactory
{
    private static readonly ConcurrentDictionary<string, ReferenceAssemblies> _cache = new();

    public static ReferenceAssemblies Get(Framework framework, string netVersion = "net8.0")
    {
        // avalonia defaults to net6.0 in original codebase if not specified, 
        // but explicit versions were also requested.
        // Special case for Avalonia fallback
        if (framework == Framework.Avalonia && netVersion == "net8.0")
        {
            // Usually we use net8.0 for everything, but let's keep net6.0 Avalonia 
            // as the default if a specific version isn't requested in existing tests.
            // Wait, existing tests explicitly used Net60Avalonia for Framework.Avalonia.
            netVersion = "net6.0";
        }

        var cacheKey = $"{framework}_{netVersion}";
        return _cache.GetOrAdd(cacheKey, _ => BuildReferenceAssemblies(framework, netVersion));
    }

    private static ReferenceAssemblies BuildReferenceAssemblies(Framework framework, string netVersion)
    {
        return framework switch
        {
            Framework.Wpf => GetBaseWindows(netVersion),
            Framework.None => GetBase(netVersion),
            Framework.Uwp => GetBaseWindows(netVersion).AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.NETCore.UniversalWindowsPlatform", "6.2.14"),
                new PackageIdentity("Microsoft.UI.Xaml", "2.8.6"),
                new PackageIdentity("Microsoft.Net.UWPCoreRuntimeSdk", "2.2.14"))),
            Framework.WinUi => GetBaseWindows(netVersion).AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.WindowsAppSDK", "1.4.231115000"),
                new PackageIdentity("Microsoft.UI.Xaml", "2.8.6"),
                new PackageIdentity("Microsoft.Windows.SDK.NET.Ref", "10.0.22621.31"))),
            Framework.Maui => GetBase(netVersion).AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.Maui.Controls.Ref.any", "7.0.101"),
                new PackageIdentity("Microsoft.Maui.Core.Ref.any", "7.0.101"))),
            Framework.Avalonia => GetBase(netVersion).AddPackages(ImmutableArray.Create(
                new PackageIdentity("Avalonia", "11.0.5"))),
            Framework.Uno => GetBase(netVersion).AddPackages(ImmutableArray.Create(
                new PackageIdentity("Uno.UI", "5.0.48"))),
            Framework.UnoWinUi => GetBase(netVersion).AddPackages(ImmutableArray.Create(
                new PackageIdentity("Uno.WinUI", "5.0.48"))),
            _ => GetBase(netVersion)
        };
    }

    private static ReferenceAssemblies GetBase(string netVersion) => netVersion switch
    {
        "net7.0" => LatestReferenceAssemblies.Net70,
        "net8.0" => LatestReferenceAssemblies.Net80,
        "net9.0" => LatestReferenceAssemblies.Net90,
        "net6.0" => ReferenceAssemblies.Net.Net60,
        _ => ReferenceAssemblies.Net.Net80
    };

    private static ReferenceAssemblies GetBaseWindows(string netVersion) => netVersion switch
    {
        "net7.0" => LatestReferenceAssemblies.Net70Windows,
        "net8.0" => LatestReferenceAssemblies.Net80Windows,
        "net9.0" => LatestReferenceAssemblies.Net90Windows,
        _ => LatestReferenceAssemblies.Net80Windows
    };
}
