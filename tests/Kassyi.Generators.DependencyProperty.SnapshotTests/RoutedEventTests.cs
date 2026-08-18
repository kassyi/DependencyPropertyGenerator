using Kassyi.Generators.DependencyProperty.Generators;
using Kassyi.Generators.Tests.Extensions;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

[TestClass]
[TestCategory(TestCategoryNames.Routed)]
public class RoutedEventTests : SnapshotTestBase
{
    [TestMethod]
    [TestCategory($"{TestCategoryNames.Routed}-001")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task RoutedEvent(Framework framework)
    {
        return CheckSourceAsync<RoutedEventGenerator>(GetHeader(framework, "Controls") + $$"""
            [RoutedEvent("TrayLeftMouseDown", RoutedEventStrategy.Bubble, WinRtEvents = true)]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Routed}-002")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Maui)]
    [DataRow(Framework.Avalonia)]
    public Task AttachedRoutedEvent(Framework framework)
    {
        return CheckSourceAsync<RoutedEventGenerator>(GetHeader(framework, "Controls") + $$"""
            [RoutedEvent("TrayLeftMouseDown", RoutedEventStrategy.Bubble, IsAttached = true)]
            public partial class MyControl : {{FrameworkTestData.GetUserControl(framework)}}
            {
            }
            """, framework);
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Routed}-003")]
    public async Task AttachedRoutedEvent_StaticClass_DoesNotDuplicatePublicModifier()
    {
        const Framework framework = Framework.Wpf;
        var source = GetHeader(framework, "Controls") + $$"""
                                                        [RoutedEvent("MouseDoubleClickEvent", RoutedEventStrategy.Bubble, IsAttached = true)]
                                                        public static partial class ImageRoutedEvents
                                                        {
                                                        }
                                                        """;
        var generated = await GenerateSourceAsync<RoutedEventGenerator>(source, framework);
        Assert.IsFalse(generated.Contains("publicpublic"));
        Assert.IsTrue(generated.Contains("public static partial class ImageRoutedEvents"));
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Routed}-004")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Avalonia)]
    public async Task RoutedEvent_WithGenericHandlerType_DoesNotProduceDuplicateGlobalPrefix(Framework framework)
    {
        var source = GetHeader(framework, "Controls") + $$"""
                                                        public delegate void MyRoutedEventHandler(object sender, global::System.EventArgs e);
                                                        [RoutedEvent<MyRoutedEventHandler>("TrayLeftMouseDown", RoutedEventStrategy.Bubble, WinRtEvents = true)]
                                                        public partial class MyControl : UserControl
                                                        {
                                                        }
                                                        """;
        var generated = await GenerateSourceAsync<RoutedEventGenerator>(source, framework);
        Assert.IsFalse(generated.Contains("global::global::"));
        Assert.IsTrue(generated.Contains("global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyRoutedEventHandler"));
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Routed}-005")]
    [DataRow(Framework.Wpf)]
    [DataRow(Framework.Uno)]
    [DataRow(Framework.UnoWinUi)]
    [DataRow(Framework.Avalonia)]
    public async Task RoutedEvent_WithTypeNamedArgument_DoesNotProduceDuplicateGlobalPrefix(Framework framework)
    {
        var source = GetHeader(framework, "Controls") + $$"""
                                                        public delegate void MyRoutedEventHandler(object sender, global::System.EventArgs e);
                                                        [RoutedEvent("TrayLeftMouseDown", RoutedEventStrategy.Bubble, Type = typeof(MyRoutedEventHandler), WinRtEvents = true)]
                                                        public partial class MyControl : UserControl
                                                        {
                                                        }
                                                        """;
        var generated = await GenerateSourceAsync<RoutedEventGenerator>(source, framework);
        Assert.IsFalse(generated.Contains("global::global::"));
        Assert.IsTrue(generated.Contains("global::Kassyi.Generators.DependencyProperty.IntegrationTests.MyRoutedEventHandler"));
    }

    [TestMethod]
    [TestCategory($"{TestCategoryNames.Routed}-006")]
    public async Task Cs0436Suppressor_SuppressesOnlyGeneratedAttributeConflicts()
    {
        var parseOptions = Microsoft.CodeAnalysis.CSharp.CSharpParseOptions.Default
            .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview);
        var references = (await Microsoft.CodeAnalysis.Testing.ReferenceAssemblies.NetFramework.Net48.Wpf.ResolveAsync(null, CancellationToken.None))
            .ToArray();
        using var projectAAssembly = CreateAndEmitCompilation(
            assemblyName: "ProjectA",
            source: """
                    using System.Runtime.CompilerServices;
                    using Kassyi.Generators.DependencyProperty;

                    using System.Windows.Controls;
                    [assembly: InternalsVisibleTo("ProjectB")]
                    namespace ProjectA;
                    internal sealed class SharedType
                    {
                    }
                    [RoutedEvent("Opened", RoutedEventStrategy.Bubble)]
                    internal partial class ProjectAControl : Control
                    {
                    }
                    """,
            references,
            parseOptions);

        var projectB = CreateCompilation(
            assemblyName: "ProjectB",
            source: """
                    using Kassyi.Generators.DependencyProperty;

                    using System.Windows.Controls;
                    namespace ProjectA
                    {
                        internal sealed class SharedType
                        {
                        }
                    }
                    namespace ProjectB
                    {
                        [RoutedEvent("Closed", RoutedEventStrategy.Bubble)]
                        internal partial class ProjectBControl : Control
                        {
                            private ProjectA.SharedType? SharedType { get; set; }
                        }
                    }
                    """,
            references.Concat([Microsoft.CodeAnalysis.MetadataReference.CreateFromStream(projectAAssembly)]).ToArray(),
            parseOptions);
        projectB = RunRoutedEventGenerator(projectB, parseOptions);
        var rawCs0436Diagnostics = projectB.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Id == "CS0436")
            .ToArray();
        Assert.IsTrue(rawCs0436Diagnostics.Any(static diagnostic =>
            diagnostic.GetMessage().Contains("RoutedEventAttribute", StringComparison.Ordinal)));
        Assert.IsTrue(rawCs0436Diagnostics.Any(static diagnostic =>
            diagnostic.GetMessage().Contains("SharedType", StringComparison.Ordinal)));
        var diagnostics = await Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzerExtensions
            .WithAnalyzers(
                projectB,
                [
                    new Suppressors.Cs0436Suppressor()
                ],
                new Microsoft.CodeAnalysis.Diagnostics.AnalyzerOptions(
                    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AdditionalText>.Empty))
            .GetAllDiagnosticsAsync();
        Assert.AreEqual(1, diagnostics.Count(static diagnostic => diagnostic.Id == "CS0436" && diagnostic.GetMessage().Contains("SharedType", StringComparison.Ordinal)));
        Assert.IsFalse(diagnostics.Any(static diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
    }
    private static Microsoft.CodeAnalysis.Compilation CreateCompilation(
        string assemblyName,
        string source,
        Microsoft.CodeAnalysis.MetadataReference[] references,
        Microsoft.CodeAnalysis.CSharp.CSharpParseOptions parseOptions)
    {
        return Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
            assemblyName: assemblyName,
            syntaxTrees:
            [
                Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source, options: parseOptions)
            ],
            references: references,
            options: new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
    }

    private static MemoryStream CreateAndEmitCompilation(
        string assemblyName,
        string source,
        Microsoft.CodeAnalysis.MetadataReference[] references,
        Microsoft.CodeAnalysis.CSharp.CSharpParseOptions parseOptions)
    {
        var compilation = CreateCompilation(assemblyName, source, references, parseOptions);
        compilation = RunRoutedEventGenerator(compilation, parseOptions);
        
        var assemblyStream = new MemoryStream();
        var emitResult = compilation.Emit(assemblyStream);
        Assert.IsTrue(emitResult.Success, string.Join(Environment.NewLine, emitResult.Diagnostics));
        assemblyStream.Position = 0;
        
        return assemblyStream;
    }

    private static Microsoft.CodeAnalysis.Compilation RunRoutedEventGenerator(
        Microsoft.CodeAnalysis.Compilation compilation,
        Microsoft.CodeAnalysis.CSharp.CSharpParseOptions parseOptions)
    {
        Microsoft.CodeAnalysis.GeneratorDriver driver = Microsoft.CodeAnalysis.CSharp.CSharpGeneratorDriver.Create(
            generators: [Microsoft.CodeAnalysis.GeneratorExtensions.AsSourceGenerator(new RoutedEventGenerator())],
            parseOptions: parseOptions);
        driver = driver
            .WithUpdatedAnalyzerConfigOptions(new DictionaryAnalyzerConfigOptionsProvider(global::Kassyi.Generators.Tests.Extensions.GlobalOptionsHelper.GetGlobalOptions(Framework.Wpf)))
            .RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _);
        return updatedCompilation;
    }
}
