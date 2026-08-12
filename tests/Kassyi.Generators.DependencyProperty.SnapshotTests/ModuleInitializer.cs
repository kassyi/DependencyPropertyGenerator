using System.Runtime.CompilerServices;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}

