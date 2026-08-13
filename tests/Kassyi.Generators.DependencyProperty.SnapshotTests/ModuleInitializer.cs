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
        VerifierSettings.AddScrubber("cs", sb =>
        {
            var text = sb.ToString();
            var tree = CSharpSyntaxTree.ParseText(text);
            var formatted = tree.GetRoot().NormalizeWhitespace().ToFullString();
            sb.Clear();
            sb.Append(formatted);
        });
    }
}

