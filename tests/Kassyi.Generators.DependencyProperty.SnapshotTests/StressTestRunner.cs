#nullable enable

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Compression;
using Kassyi.Generators.DependencyProperty.Generators;
using Kassyi.Generators.Tests.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public enum PlatformType
{
    Wpf,
    Uwp,
    WinUI,
    Avalonia,
    Uno,
    Maui
}

public record StressTestTarget(
    string Name,
    string RepoUrl,
    string ExtractDirName,
    string SourceRelativePath,
    PlatformType Platform
);

public static class PlatformReferenceAssemblies
{
    public static ReferenceAssemblies Get(PlatformType platform)
    {
        return platform switch
        {
            PlatformType.Wpf => ReferenceAssemblies.NetFramework.Net48.Wpf,
            PlatformType.WinUI => ReferenceAssemblies.Net.Net80.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.WindowsAppSDK", "1.5.240311000"))),
            PlatformType.Avalonia => ReferenceAssemblies.Net.Net80.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Avalonia", "11.0.10"),
                new PackageIdentity("Avalonia.Desktop", "11.0.10"))),
            PlatformType.Maui => ReferenceAssemblies.Net.Net80.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.Maui.Controls", "8.0.21"),
                new PackageIdentity("Microsoft.Maui.Controls.Core", "8.0.21"))),
            PlatformType.Uno => ReferenceAssemblies.Net.Net80.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Uno.WinUI", "5.2.161"))),
            PlatformType.Uwp => ReferenceAssemblies.NetStandard.NetStandard20.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.NETCore.UniversalWindowsPlatform", "6.2.14"))),
            _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
        };
    }
}

public static class PlatformHelper
{
    public static string GetBaseClass(PlatformType platform) => platform switch
    {
        PlatformType.Avalonia => "global::Avalonia.AvaloniaObject",
        PlatformType.Maui => "global::Microsoft.Maui.Controls.BindableObject",
        _ => "global::System.Windows.DependencyObject"
    };

    public static string GetDefineConstants(PlatformType platform) => platform switch
    {
        PlatformType.Avalonia => "HAS_AVALONIA",
        PlatformType.Maui => "HAS_MAUI",
        PlatformType.Uno => "HAS_UNO;HAS_WINUI",
        PlatformType.Uwp => "WINDOWS_UWP",
        _ => ""
    };

    public static string GetRegistrationMethod(PlatformType platform) => platform switch
    {
        PlatformType.Avalonia => "AvaloniaProperty.Register",
        PlatformType.Maui => "BindableProperty.Create",
        _ => "DependencyProperty.Register"
    };
}

public static class RepositoryDownloader
{
    public static async Task<string> EnsureRepositoryAsync(StressTestTarget target)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "DependencyPropertyGenerator_StressTests");
        var extractDir = Path.Combine(tempDir, target.ExtractDirName);
        
        if (!Directory.Exists(extractDir))
        {
            Directory.CreateDirectory(tempDir);
            var zipPath = Path.Combine(tempDir, $"{target.ExtractDirName}.zip");
            
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(target.RepoUrl);
            response.EnsureSuccessStatusCode();
            
            await using var fs = new FileStream(zipPath, FileMode.Create);
            await response.Content.CopyToAsync(fs);
            fs.Close();
            
            ZipFile.ExtractToDirectory(zipPath, tempDir, overwriteFiles: true);
        }

        var srcDir = Path.Combine(extractDir, target.SourceRelativePath);
        Assert.IsTrue(Directory.Exists(srcDir), $"Source directory not found at {srcDir}");
        return srcDir;
    }
}

public record StressTestExecutionResult(
    int GeneratedTreesCount,
    bool HasRegistrationMethod,
    IReadOnlyCollection<Diagnostic> DriverDiagnostics,
    IReadOnlyCollection<Diagnostic> GeneratorDiagnostics
);

public static class StressTestRunner
{
    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Preview);
    private static readonly UsingDirectiveSyntax GeneratorUsingDirective =
        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Kassyi.Generators.DependencyProperty"))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

    public static async Task RunAsync(StressTestTarget target)
    {
        var srcDir = await RepositoryDownloader.EnsureRepositoryAsync(target);
        var syntaxTrees = await LoadAndInjectSyntaxTreesAsync(srcDir, target.Platform);
        var references = await ResolveReferencesAsync(target.Platform);

        var result = await RunGeneratorInChunksAsync(target, syntaxTrees, references);

        AssertGeneratorResults(target, result);
    }

    private static async Task<SyntaxTree[]> LoadAndInjectSyntaxTreesAsync(string srcDir, PlatformType platform)
    {
        var csFiles = Directory.GetFiles(srcDir, "*.cs", SearchOption.AllDirectories);
        Assert.IsTrue(csFiles.Length > 0, $"No C# source files found in {srcDir}");

        var syntaxTreesBag = new ConcurrentBag<SyntaxTree>();
        var injectedClassesCount = 0;

        await Parallel.ForEachAsync(csFiles, async (file, ct) =>
        {
            var sourceText = await File.ReadAllTextAsync(file, ct);
            var tree = CSharpSyntaxTree.ParseText(sourceText, ParseOptions, path: file, cancellationToken: ct);
            var root = await tree.GetRootAsync(ct);

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            if (classes.Count > 0)
            {
                var rewriter = new InjectAttributesRewriter(platform);
                root = rewriter.Visit(root);
                var compilationUnit = ((CompilationUnitSyntax)root).AddUsings(GeneratorUsingDirective);
                tree = tree.WithRootAndOptions(compilationUnit, tree.Options);
                Interlocked.Add(ref injectedClassesCount, rewriter.InjectedClassCount);
            }

            syntaxTreesBag.Add(tree);
        });

        Assert.IsTrue(injectedClassesCount > 0, $"Could not inject any test properties. No partial classes found.");
        return syntaxTreesBag.ToArray();
    }

    private static async Task<IReadOnlyList<MetadataReference>> ResolveReferencesAsync(PlatformType platform)
    {
        var referenceAssemblies = PlatformReferenceAssemblies.Get(platform);
        var references = await referenceAssemblies.ResolveAsync(null, CancellationToken.None);
        return references.ToList();
    }

    private static async Task<StressTestExecutionResult> RunGeneratorInChunksAsync(
        StressTestTarget target,
        SyntaxTree[] syntaxTrees,
        IReadOnlyList<MetadataReference> references)
    {
        var processorCount = Environment.ProcessorCount;
        var chunkSize = Math.Max(1, (int)Math.Ceiling((double)syntaxTrees.Length / processorCount));
        var chunks = syntaxTrees.Chunk(chunkSize).ToArray();

        var driverDiagnostics = new ConcurrentBag<Diagnostic>();
        var generatorDiagnostics = new ConcurrentBag<Diagnostic>();
        var generatedTreesCount = 0;
        var hasRegistrationMethodFlag = 0;

        var dummySyntax = CreateDummyTestClassSyntax(target.Platform);
        var registrationMethod = PlatformHelper.GetRegistrationMethod(target.Platform);

        await Parallel.ForEachAsync(chunks, (chunk, ct) =>
        {
            var chunkTrees = chunk.ToList();
            chunkTrees.Add(dummySyntax);

            var compilation = CSharpCompilation.Create(
                assemblyName: $"{target.Name}_StressTest_{Guid.NewGuid():N}",
                syntaxTrees: chunkTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var driver = CreateGeneratorDriver(target.Platform);
            driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
            var runResult = driver.GetRunResult();

            foreach (var d in runResult.Diagnostics)
            {
                driverDiagnostics.Add(d);
            }

            foreach (var d in diagnostics)
            {
                if (d.Id.StartsWith("DPG", StringComparison.Ordinal) || d.Id.StartsWith("AD", StringComparison.Ordinal))
                {
                    generatorDiagnostics.Add(d);
                }
            }

            Interlocked.Add(ref generatedTreesCount, runResult.GeneratedTrees.Length);

            var generatedCode = string.Join("\n", runResult.GeneratedTrees.Select(static t => t.ToString()));
            if (generatedCode.Contains(registrationMethod))
            {
                Interlocked.Exchange(ref hasRegistrationMethodFlag, 1);
            }

            return ValueTask.CompletedTask;
        });

        return new StressTestExecutionResult(
            GeneratedTreesCount: generatedTreesCount,
            HasRegistrationMethod: hasRegistrationMethodFlag == 1,
            DriverDiagnostics: driverDiagnostics.ToArray(),
            GeneratorDiagnostics: generatorDiagnostics.ToArray()
        );
    }

    private static GeneratorDriver CreateGeneratorDriver(PlatformType platform)
    {
        var generators = new IIncrementalGenerator[]
        {
            new DependencyPropertyGenerator(),
            new AttachedDependencyPropertyGenerator(),
            new RoutedEventGenerator(),
            new StaticConstructorGenerator()
        };

        var driver = CSharpGeneratorDriver.Create(
            generators: generators.Select(GeneratorExtensions.AsSourceGenerator).ToArray(),
            parseOptions: ParseOptions
        );

        return driver.WithUpdatedAnalyzerConfigOptions(new DictionaryAnalyzerConfigOptionsProvider(
            new Dictionary<string, string>
            {
                { "build_property.UseWPF", platform == PlatformType.Wpf ? "true" : "false" },
                { "build_property.UseWinUI", platform == PlatformType.WinUI ? "true" : "false" },
                { "build_property.RecognizeFramework_DefineConstants", PlatformHelper.GetDefineConstants(platform) },
                { "build_property.RecognizeFramework_Version", "0.0.0.0" }
            }));
    }

    private static SyntaxTree CreateDummyTestClassSyntax(PlatformType platform)
    {
        var baseClass = PlatformHelper.GetBaseClass(platform);
        return CSharpSyntaxTree.ParseText($$"""
            namespace StressTestDummy
            {
                [global::Kassyi.Generators.DependencyProperty.DependencyProperty("StressTestProperty", typeof(int))]
                [global::Kassyi.Generators.DependencyProperty.AttachedDependencyProperty("StressTestAttached", typeof(string), BrowsableForType = typeof({{baseClass}}))]
                public partial class DummyControl : {{baseClass}}
                {
                }
            }
            """, ParseOptions, path: "DummyControl.cs");
    }

    private static void AssertGeneratorResults(StressTestTarget target, StressTestExecutionResult result)
    {
        Assert.IsTrue(result.HasRegistrationMethod,
            $"No property registration method calls found in the generated code for {target.Platform}. This suggests a complete failure to generate semantics.");
        Assert.IsTrue(result.GeneratedTreesCount > 0, "Expected at least 1 generated source file, but got 0.");

        var unexpectedDriverDiagnostics = result.DriverDiagnostics
            .Where(static d => !d.GetMessage().Contains("DPG0002"))
            .ToList();
        Assert.AreEqual(0, unexpectedDriverDiagnostics.Count,
            $"Generator driver emitted unexpected diagnostics: {string.Join("; ", unexpectedDriverDiagnostics.Select(static d => $"{d.Id}: {d.GetMessage()}"))}");

        var unexpectedGeneratorDiagnostics = result.GeneratorDiagnostics
            .Where(static d => d.Id != "DPG0002" && !d.GetMessage().Contains("DPG0002"))
            .ToList();
        Assert.AreEqual(0, unexpectedGeneratorDiagnostics.Count,
            $"Generator emitted unexpected diagnostics: {string.Join("; ", unexpectedGeneratorDiagnostics.Select(static d => $"{d.Id}: {d.GetMessage()}"))}");
    }

    private sealed class InjectAttributesRewriter : CSharpSyntaxRewriter
    {
        private static int s_classCounter;
        private readonly string _baseClass;
        private int _injectedClassCount;

        public InjectAttributesRewriter(PlatformType platform)
        {
            _baseClass = PlatformHelper.GetBaseClass(platform);
        }

        public int InjectedClassCount => _injectedClassCount;

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var id = Interlocked.Increment(ref s_classCounter);
            _injectedClassCount++;

            var injectedAttributes = SyntaxFactory.ParseCompilationUnit($$"""
                [global::Kassyi.Generators.DependencyProperty.DependencyProperty("StressTestProperty_{{id}}", typeof(int))]
                [global::Kassyi.Generators.DependencyProperty.AttachedDependencyProperty("StressTestAttached_{{id}}", typeof(string), BrowsableForType = typeof({{_baseClass}}))]
                class Dummy {}
                """)
                .DescendantNodes().OfType<ClassDeclarationSyntax>().First().AttributeLists.ToArray();

            var newNode = node.AddAttributeLists(injectedAttributes);
            if (!newNode.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                newNode = newNode.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            }
            
            return base.VisitClassDeclaration(newNode);
        }
    }
}
