using System;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis.Testing;

namespace Kassyi.Generators.Tests.Extensions;

[Obsolete("Use ReferenceAssembliesFactory.Get(Framework, netVersion) instead.")]
public static class FrameworkReferenceAssemblies
{
    public static ReferenceAssemblies Net70Uwp => ReferenceAssembliesFactory.Get(Framework.Uwp, "net7.0");
    public static ReferenceAssemblies Net80Uwp => ReferenceAssembliesFactory.Get(Framework.Uwp, "net8.0");
    
    public static ReferenceAssemblies Net70WinUi => ReferenceAssembliesFactory.Get(Framework.WinUi, "net7.0");
    public static ReferenceAssemblies Net80WinUi => ReferenceAssembliesFactory.Get(Framework.WinUi, "net8.0");
    
    public static ReferenceAssemblies Net70Maui => ReferenceAssembliesFactory.Get(Framework.Maui, "net7.0");
    public static ReferenceAssemblies Net80Maui => ReferenceAssembliesFactory.Get(Framework.Maui, "net8.0");
    
    public static ReferenceAssemblies Net60Avalonia => ReferenceAssembliesFactory.Get(Framework.Avalonia, "net6.0");
    public static ReferenceAssemblies Net70Avalonia => ReferenceAssembliesFactory.Get(Framework.Avalonia, "net7.0");
    public static ReferenceAssemblies Net80Avalonia => ReferenceAssembliesFactory.Get(Framework.Avalonia, "net8.0");
    
    public static ReferenceAssemblies Net70Uno4 => ReferenceAssembliesFactory.Get(Framework.Uno, "net7.0");
    public static ReferenceAssemblies Net70Uno4WinUi => ReferenceAssembliesFactory.Get(Framework.UnoWinUi, "net7.0");
    public static ReferenceAssemblies Net70Uno => ReferenceAssembliesFactory.Get(Framework.Uno, "net7.0");
    public static ReferenceAssemblies Net70UnoWinUi => ReferenceAssembliesFactory.Get(Framework.UnoWinUi, "net7.0");
    
    public static ReferenceAssemblies Net80Uno4 => ReferenceAssembliesFactory.Get(Framework.Uno, "net8.0");
    public static ReferenceAssemblies Net80Uno4WinUi => ReferenceAssembliesFactory.Get(Framework.UnoWinUi, "net8.0");
    public static ReferenceAssemblies Net80Uno => ReferenceAssembliesFactory.Get(Framework.Uno, "net8.0");
    public static ReferenceAssemblies Net80UnoWinUi => ReferenceAssembliesFactory.Get(Framework.UnoWinUi, "net8.0");
}
