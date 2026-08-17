using System.Collections.Generic;
using Kassyi.Generators.Extensions;

namespace Kassyi.Generators.Tests.Extensions;

public static class GlobalOptionsHelper
{
    public static Dictionary<string, string> GetGlobalOptions(Framework framework)
    {
        var options = new Dictionary<string, string>
        {
            ["build_property.RecognizeFramework_Version"] = "0.0.0.0"
        };

        switch (framework)
        {
            case Framework.Wpf: options["build_property.UseWPF"] = "true"; break;
            case Framework.WinUi: options["build_property.UseWinUI"] = "true"; break;
            case Framework.Maui: options["build_property.UseMaui"] = "true"; break;
            case Framework.Uwp: options["build_property.RecognizeFramework_DefineConstants"] = "WINDOWS_UWP"; break;
            case Framework.Uno: options["build_property.RecognizeFramework_DefineConstants"] = "HAS_UNO"; break;
            case Framework.UnoWinUi: options["build_property.RecognizeFramework_DefineConstants"] = "HAS_UNO;HAS_WINUI"; break;
            case Framework.Avalonia: options["build_property.RecognizeFramework_DefineConstants"] = "HAS_AVALONIA"; break;
        }

        return options;
    }
}
