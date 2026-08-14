#nullable enable

using System.IO.Compression;
using Kassyi.Generators.DependencyProperty.Generators;
using Kassyi.Generators.Tests.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
public class RealWorldStressTests
{
    private const string WpfUiRepoUrl = "https://github.com/lepoco/wpfui/archive/refs/heads/main.zip";
    
    [TestMethod]
    [TestCategory("Stress")]
    public async Task WpfUi_StressTest_NoDiagnostics()
    {
        var srcDir = await EnsureWpfUiRepositoryAsync();

        var csFiles = Directory.GetFiles(srcDir, "*.cs", SearchOption.AllDirectories);
        Assert.IsTrue(csFiles.Length > 0, "No C# source files found.");

        var syntaxTrees = new List<SyntaxTree>(csFiles.Length + 1);
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        var rewriter = new InjectAttributesRewriter();
        var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Kassyi.Generators.DependencyProperty"))
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        foreach (var file in csFiles)
        {
            var sourceText = await File.ReadAllTextAsync(file);
            var tree = CSharpSyntaxTree.ParseText(sourceText, parseOptions, path: file);
            var root = tree.GetRoot();

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            if (classes.Count > 0)
            {
                root = rewriter.Visit(root);
                var compilationUnit = ((CompilationUnitSyntax)root).AddUsings(usingDirective);
                tree = tree.WithRootAndOptions(compilationUnit, tree.Options);
            }

            syntaxTrees.Add(tree);
        }
        
        Assert.IsTrue(rewriter.InjectedClassCount > 0, "Could not inject any test properties. No partial classes found.");

        // Add a clean dummy class to guarantee verification of generator behavior
        var dummyTestClassSyntax = CSharpSyntaxTree.ParseText("""
            namespace StressTestDummy
            {
                [global::Kassyi.Generators.DependencyProperty.DependencyProperty("StressTestProperty", typeof(int))]
                [global::Kassyi.Generators.DependencyProperty.AttachedDependencyProperty("StressTestAttached", typeof(string), BrowsableForType = typeof(global::System.Windows.DependencyObject))]
                [global::Kassyi.Generators.DependencyProperty.RoutedEvent("StressTestEvent", global::Kassyi.Generators.DependencyProperty.RoutedEventStrategy.Bubble, Type = typeof(global::System.Windows.RoutedEventHandler))]
                public partial class DummyControl : global::System.Windows.DependencyObject
                {
                }
            }
            """, parseOptions, path: "DummyControl.cs");
        syntaxTrees.Add(dummyTestClassSyntax);

        var references = await ReferenceAssemblies.NetFramework.Net48.Wpf.ResolveAsync(null, CancellationToken.None);
        var refList = references.ToList();
        
        var compilation = CSharpCompilation.Create(
            assemblyName: "WpfUi_StressTest",
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
                { "build_property.UseWPF", "true" },
                { "build_property.RecognizeFramework_Version", "0.0.0.0" }
            }));

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);

        var runResult = driver.GetRunResult();
        var generatorDiagnostics = diagnostics.Where(static d => d.Id.StartsWith("DPG", StringComparison.Ordinal) || d.Id.StartsWith("AD", StringComparison.Ordinal)).ToList();
        var compilerErrors = diagnostics.Where(static d => d.Severity == DiagnosticSeverity.Error).ToList();
        var driverDiagnostics = runResult.Diagnostics.ToList();

        // 1. Verify that attributes were successfully injected into hundreds of classes
        Assert.AreEqual(177, rewriter.InjectedClassCount, $"Expected exactly 177 injected classes, but got {rewriter.InjectedClassCount}.");

        // 2. Verify that generators produced hundreds of source files across all targets
        Assert.AreEqual(541, runResult.GeneratedTrees.Length, $"Expected exactly 541 generated source files, but got {runResult.GeneratedTrees.Length}.");

        // 3. Verify that all generated source files contain non-empty code
        Assert.IsTrue(runResult.GeneratedTrees.All(static tree => !string.IsNullOrWhiteSpace(tree.ToString())), "One or more generated source files were unexpectedly empty.");

        // 4. Assert that no internal generator crashes, compiler errors, or driver diagnostics occurred
        Assert.AreEqual(0, driverDiagnostics.Count, $"Generator driver emitted unexpected diagnostics: {string.Join("; ", driverDiagnostics.Select(static d => $"{d.Id}: {d.GetMessage()}"))}");
        Assert.AreEqual(0, generatorDiagnostics.Count, $"Generator emitted unexpected diagnostics: {string.Join("; ", generatorDiagnostics.Select(static d => $"{d.Id}: {d.GetMessage()}"))}");
        Assert.AreEqual(0, compilerErrors.Count, $"Unexpected compilation errors: {string.Join("; ", compilerErrors.Select(static d => $"{d.Id}: {d.GetMessage()}"))}");
    }

    private static async Task<string> EnsureWpfUiRepositoryAsync()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "DependencyPropertyGenerator_StressTests");
        var extractDir = Path.Combine(tempDir, "wpfui-main");
        
        if (!Directory.Exists(extractDir))
        {
            Directory.CreateDirectory(tempDir);
            var zipPath = Path.Combine(tempDir, "wpfui-main.zip");
            
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(WpfUiRepoUrl);
            response.EnsureSuccessStatusCode();
            
            await using var fs = new FileStream(zipPath, FileMode.Create);
            await response.Content.CopyToAsync(fs);
            fs.Close();
            
            ZipFile.ExtractToDirectory(zipPath, tempDir, overwriteFiles: true);
        }

        var srcDir = Path.Combine(extractDir, "src", "Wpf.Ui");
        Assert.IsTrue(Directory.Exists(srcDir), $"Source directory not found at {srcDir}");
        return srcDir;
    }

    private sealed class InjectAttributesRewriter : CSharpSyntaxRewriter
    {
        private static readonly AttributeListSyntax[] s_injectedAttributes = SyntaxFactory.ParseCompilationUnit("""
            [global::Kassyi.Generators.DependencyProperty.DependencyProperty("StressTestProperty", typeof(int))]
            [global::Kassyi.Generators.DependencyProperty.AttachedDependencyProperty("StressTestAttached", typeof(string), BrowsableForType = typeof(global::System.Windows.DependencyObject))]
            [global::Kassyi.Generators.DependencyProperty.RoutedEvent("StressTestEvent", global::Kassyi.Generators.DependencyProperty.RoutedEventStrategy.Bubble, Type = typeof(global::System.Windows.RoutedEventHandler))]
            class Dummy {}
            """)
            .DescendantNodes().OfType<ClassDeclarationSyntax>().First().AttributeLists.ToArray();

        private readonly HashSet<string> _injectedClasses = new(StringComparer.Ordinal);

        public int InjectedClassCount => _injectedClasses.Count;

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var qualifiedClassName = GetQualifiedClassName(node);
            if (!_injectedClasses.Add(qualifiedClassName))
            {
                // Already injected attributes into a partial declaration of this qualified class.
                return base.VisitClassDeclaration(node);
            }

            var newNode = node.AddAttributeLists(s_injectedAttributes);
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
