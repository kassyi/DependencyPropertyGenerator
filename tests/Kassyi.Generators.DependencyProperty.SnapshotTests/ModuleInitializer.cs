using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
        var dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir) && !Directory.GetFiles(dir, "*.sln").Any())
        {
            var parent = Directory.GetParent(dir);
            if (parent == null) break;
            dir = parent.FullName;
        }
        if (!string.IsNullOrEmpty(dir))
        {
            var solutionDir = dir.TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
            VerifierSettings.AddScrubber(builder => builder.Replace(solutionDir, "{SolutionDirectory}"));
        }
    }
}

