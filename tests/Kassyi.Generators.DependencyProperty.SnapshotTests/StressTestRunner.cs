#nullable enable

using System.Collections.Immutable;
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
        // 妥協せず、Microsoft.CodeAnalysis.Testing を利用して NuGet パッケージ群から
        // フレームワークのメタデータを取得・解決する
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

public static class StressTestRunner
{
    public static async Task RunAsync(StressTestTarget target)
    {
        var srcDir = await RepositoryDownloader.EnsureRepositoryAsync(target);

        var csFiles = Directory.GetFiles(srcDir, "*.cs", SearchOption.AllDirectories);
        Assert.IsTrue(csFiles.Length > 0, $"No C# source files found in {srcDir}");

        var syntaxTreesBag = new System.Collections.Concurrent.ConcurrentBag<SyntaxTree>();
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        var injectedClasses = new System.Collections.Concurrent.ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
        
        var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Kassyi.Generators.DependencyProperty"))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        await Parallel.ForEachAsync(csFiles, async (file, ct) =>
        {
            var sourceText = await File.ReadAllTextAsync(file, ct);
            var tree = CSharpSyntaxTree.ParseText(sourceText, parseOptions, path: file, cancellationToken: ct);
            var root = await tree.GetRootAsync(ct);

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            if (classes.Count > 0)
            {
                var rewriter = new InjectAttributesRewriter(target.Platform, injectedClasses);
                root = rewriter.Visit(root);
                var compilationUnit = ((CompilationUnitSyntax)root).AddUsings(usingDirective);
                tree = tree.WithRootAndOptions(compilationUnit, tree.Options);
            }

            syntaxTreesBag.Add(tree);
        });

        var syntaxTrees = syntaxTreesBag.ToList();

        Assert.IsTrue(injectedClasses.Count > 0, $"Could not inject any test properties into {target.Name}. No partial classes found.");

        // Add a clean dummy class to guarantee verification of generator behavior
        var baseClass = target.Platform switch
        {
            PlatformType.Avalonia => "global::Avalonia.AvaloniaObject",
            PlatformType.Maui => "global::Microsoft.Maui.Controls.BindableObject",
            _ => "global::System.Windows.DependencyObject" // WPF, WinUI, UWP, Uno
        };

        var dummyTestClassSyntax = CSharpSyntaxTree.ParseText($$"""
            namespace StressTestDummy
            {
                [global::Kassyi.Generators.DependencyProperty.DependencyProperty("StressTestProperty", typeof(int))]
                [global::Kassyi.Generators.DependencyProperty.AttachedDependencyProperty("StressTestAttached", typeof(string), BrowsableForType = typeof({{baseClass}}))]
                public partial class DummyControl : {{baseClass}}
                {
                }
            }
            """, parseOptions, path: "DummyControl.cs");
        syntaxTrees.Add(dummyTestClassSyntax);

        var referenceAssemblies = PlatformReferenceAssemblies.Get(target.Platform);
        var references = await referenceAssemblies.ResolveAsync(null, CancellationToken.None);
        var refList = references.ToList();

        var compilation = CSharpCompilation.Create(
            assemblyName: $"{target.Name}_StressTest",
            syntaxTrees: syntaxTrees,
            references: refList,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generators = new IIncrementalGenerator[]
        {
            new DependencyPropertyGenerator(),
            new AttachedDependencyPropertyGenerator(),
            new RoutedEventGenerator(),
            new StaticConstructorGenerator()
        };

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: generators.Select(GeneratorExtensions.AsSourceGenerator).ToArray(),
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview)
        );

        driver = driver.WithUpdatedAnalyzerConfigOptions(new DictionaryAnalyzerConfigOptionsProvider(
            new Dictionary<string, string>
            {
                { "build_property.UseWPF", target.Platform == PlatformType.Wpf ? "true" : "false" },
                { "build_property.UseWinUI", target.Platform == PlatformType.WinUI || target.Platform == PlatformType.Uno ? "true" : "false" },
                { "build_property.UseAvalonia", target.Platform == PlatformType.Avalonia ? "true" : "false" },
                { "build_property.UseMAUI", target.Platform == PlatformType.Maui ? "true" : "false" },
                { "build_property.UseUWP", target.Platform == PlatformType.Uwp ? "true" : "false" },
                { "build_property.RecognizeFramework_Version", "0.0.0.0" }
            }));

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);

        var runResult = driver.GetRunResult();
        var generatorDiagnostics = diagnostics.Where(static d => d.Id.StartsWith("DPG", StringComparison.Ordinal) || d.Id.StartsWith("AD", StringComparison.Ordinal)).ToList();
        var driverDiagnostics = runResult.Diagnostics.ToList();

        // 1. Verify that all generated source files contain non-empty code
        Assert.IsTrue(runResult.GeneratedTrees.All(static tree => !string.IsNullOrWhiteSpace(tree.ToString())), "One or more generated source files were unexpectedly empty.");

        // 2. Verify that some property registration code is actually generated (Semantic Oracle)
        var fullGeneratedCode = string.Join("\n", runResult.GeneratedTrees.Select(static t => t.ToString()));
        var hasRegistrationMethod = target.Platform switch
        {
            PlatformType.Avalonia => fullGeneratedCode.Contains("AvaloniaProperty.Register"),
            PlatformType.Maui => fullGeneratedCode.Contains("BindableProperty.Create"),
            _ => fullGeneratedCode.Contains("DependencyProperty.Register")
        };
        Assert.IsTrue(hasRegistrationMethod, $"No property registration method calls found in the generated code for {target.Platform}. This suggests a complete failure to generate semantics.");

        // 3. Assert that no internal generator crashes or driver diagnostics occurred
        Assert.AreEqual(0, driverDiagnostics.Count, $"Generator driver emitted unexpected diagnostics: {string.Join("; ", driverDiagnostics.Select(static d => $"{d.Id}: {d.GetMessage()}"))}");
        Assert.AreEqual(0, generatorDiagnostics.Count, $"Generator emitted unexpected diagnostics: {string.Join("; ", generatorDiagnostics.Select(static d => $"{d.Id}: {d.GetMessage()}"))}");
    }

    private sealed class InjectAttributesRewriter : CSharpSyntaxRewriter
    {
        private readonly AttributeListSyntax[] _injectedAttributes;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, byte> _injectedClasses;

        public InjectAttributesRewriter(PlatformType platform, System.Collections.Concurrent.ConcurrentDictionary<string, byte> injectedClasses)
        {
            _injectedClasses = injectedClasses;
            var baseClass = platform switch
            {
                PlatformType.Avalonia => "global::Avalonia.AvaloniaObject",
                PlatformType.Maui => "global::Microsoft.Maui.Controls.BindableObject",
                _ => "global::System.Windows.DependencyObject"
            };

            _injectedAttributes = SyntaxFactory.ParseCompilationUnit($$"""
                [global::Kassyi.Generators.DependencyProperty.DependencyProperty("StressTestProperty", typeof(int))]
                [global::Kassyi.Generators.DependencyProperty.AttachedDependencyProperty("StressTestAttached", typeof(string), BrowsableForType = typeof({{baseClass}}))]
                class Dummy {}
                """)
                .DescendantNodes().OfType<ClassDeclarationSyntax>().First().AttributeLists.ToArray();
        }

        public int InjectedClassCount => _injectedClasses.Count;

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var qualifiedClassName = GetQualifiedClassName(node);
            if (!_injectedClasses.TryAdd(qualifiedClassName, 0))
            {
                // Already injected attributes into a partial declaration of this qualified class.
                return base.VisitClassDeclaration(node);
            }

            var newNode = node.AddAttributeLists(_injectedAttributes);
            if (!newNode.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                newNode = newNode.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            }
            
            return base.VisitClassDeclaration(newNode);
        }

        private static string GetQualifiedClassName(ClassDeclarationSyntax node)
        {
            var sb = new System.Text.StringBuilder();

            var namespaces = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Reverse();
            foreach (var ns in namespaces)
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }
                sb.Append(ns.Name);
            }

            var enclosingClasses = node.Ancestors().OfType<ClassDeclarationSyntax>().Reverse();
            foreach (var parent in enclosingClasses)
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }
                sb.Append(parent.Identifier.Text);
                if (parent.TypeParameterList != null)
                {
                    sb.Append(parent.TypeParameterList);
                }
            }

            if (sb.Length > 0)
            {
                sb.Append('.');
            }
            sb.Append(node.Identifier.Text);
            if (node.TypeParameterList != null)
            {
                sb.Append(node.TypeParameterList);
            }

            return sb.ToString();
        }
    }
}
