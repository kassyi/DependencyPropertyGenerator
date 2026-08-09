using System.Runtime.CompilerServices;
using Kassyi.Generators.DependencyProperty.Generators;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}

