#nullable enable

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Kassyi.Generators.DependencyProperty.Generators;
using Kassyi.Generators.Tests.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public abstract partial class SnapshotTestBase : VerifyBase
{
    protected static string GetHeader(
        Framework framework,
        bool nullable,
        bool @namespace,
        params string[] values)
    {
        var prefix = framework switch
        {
            Framework.WinUi or Framework.UnoWinUi => "Microsoft.UI.Xaml",
            Framework.Uwp or Framework.Uno => "Windows.UI.Xaml",
            Framework.Avalonia => "Avalonia",
            Framework.Maui => "Microsoft.Maui",
            _ => "System.Windows",
        };

        var usings = string.Join(
            Environment.NewLine,
            values.Select(v => v switch
            {
                { Length: 0 } or null => $"using {prefix};",
                _ when v.StartsWith("System") => $"using {v};",
                _ => $"using {prefix}.{v};"
            }));

        return $"""
                {usings}
                using Kassyi.Generators.DependencyProperty;

                #nullable {(nullable ? "enable" : "disable")}
                {(@namespace ? "namespace Kassyi.Generators.DependencyProperty.IntegrationTests;" : string.Empty)}
                """;
    }

    protected static string GetHeader(
        Framework framework,
        params string[] values) =>
        GetHeader(framework, nullable: true, @namespace: true, values);


    protected static ReferenceAssemblies GetReferenceAssemblies(Framework framework) => framework switch
    {
        Framework.None or Framework.Wpf => ReferenceAssemblies.NetFramework.Net48.Wpf,
        Framework.Avalonia => ReferenceAssembliesFactory.Get(framework, "net6.0"),
        Framework.Maui => ReferenceAssembliesFactory.Get(framework, "net7.0"),
        _ => ReferenceAssembliesFactory.Get(framework, "net8.0")
    };

    private static string ApplyFrameworkReplacements(string source, Framework framework)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var rewriter = new FrameworkSyntaxRewriter(framework);
        var newSource = rewriter.Visit(tree.GetRoot()).ToFullString();

        if (framework == Framework.Avalonia)
        {
            newSource = newSource.Replace("static partial class", "partial class");
        }
        else if (framework == Framework.Maui)
        {
            newSource = Regex.Replace(newSource, @"\busing Microsoft\.Maui\.(?:Input|Controls|Media);\r?\n?", string.Empty);
            newSource = $"""
                         using Microsoft.Maui.Controls;
                         {newSource}
                         """;
        }

        return newSource;
    }

    protected static async Task<(Compilation Compilation, GeneratorDriver Driver)> CreateCompilationAndDriverAsync<T>(
        string source,
        Framework framework,
        CancellationToken cancellationToken,
        params IIncrementalGenerator[] additionalGenerators)
        where T : IIncrementalGenerator, new()
    {
        var processedSource = ApplyFrameworkReplacements(source, framework);
        var references = await GetReferenceAssemblies(framework).ResolveAsync(null, cancellationToken);

        var compilation = (Compilation)CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [
                CSharpSyntaxTree.ParseText(processedSource, options: new CSharpParseOptions(LanguageVersion.Preview),
                    cancellationToken: cancellationToken),
            ],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new T();
        IIncrementalGenerator[] allGenerators = generator switch
        {
            WeakEventGenerator or RoutedEventGenerator or StaticConstructorGenerator => [generator, .. additionalGenerators],
            _ when !additionalGenerators.Any(static x => x is StaticConstructorGenerator) => [generator, .. additionalGenerators, new StaticConstructorGenerator()],
            _ => [generator, .. additionalGenerators]
        };

        var driver = CSharpGeneratorDriver.Create(
                generators: allGenerators.Select(GeneratorExtensions.AsSourceGenerator).ToArray(),
                parseOptions: CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview))
            .WithUpdatedAnalyzerConfigOptions(new DictionaryAnalyzerConfigOptionsProvider(GlobalOptionsHelper.GetGlobalOptions(framework)));

        return (compilation, driver);
    }

    protected async Task CheckSourceAsync<T>(
        string source,
        Framework framework,
        bool skipE2EValidation = false,
        [CallerMemberName] string? callerName = null,
        CancellationToken cancellationToken = default,
        params IIncrementalGenerator[] additionalGenerators)
        where T : IIncrementalGenerator, new()
    {
        var (compilation, driver) = await CreateCompilationAndDriverAsync<T>(source, framework, cancellationToken, additionalGenerators);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var generatorDiagnostics, cancellationToken);
        var diagnostics = compilation.GetDiagnostics(cancellationToken).Concat(generatorDiagnostics);
        var diagnosticsArray = ImmutableArray.CreateRange(diagnostics);

        var generatedSyntaxTrees = driver.GetRunResult().Results
            .SelectMany(static result => result.GeneratedSources)
            .Select(static generatedSource => generatedSource.SyntaxTree)
            .ToArray();

        E2EAssertionPipeline.Verify(source, generatedSyntaxTrees, framework, compilation, diagnosticsArray, callerName ?? string.Empty, skipE2EValidation);

        await Task.WhenAll(
            Verify(diagnosticsArray.ToSnapshotModels())
                .UseDirectory($"Snapshots/{callerName}/{framework:G}")
                .UseTypeName("Tests")
                .UseTextForParameters("Diagnostics"),
            Verify(driver)
                .UseDirectory($"Snapshots/{callerName}/{framework:G}")
                .UseFileName("_"));
    }

    protected static async Task<string> GenerateSourceAsync<T>(
        string source,
        Framework framework,
        CancellationToken cancellationToken = default)
        where T : IIncrementalGenerator, new()
    {
        var (compilation, driver) = await CreateCompilationAndDriverAsync<T>(source, framework, cancellationToken);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _, cancellationToken);

        return string.Join(
            Environment.NewLine,
            driver.GetRunResult().Results
                .SelectMany(static result => result.GeneratedSources)
                .Select(static generatedSource => generatedSource.SourceText.ToString()));
    }
}

internal static class DiagnosticExtensions
{
    internal static IReadOnlyList<DiagnosticSnapshot> ToSnapshotModels(this IEnumerable<Diagnostic> diagnostics)
    {
        return [.. diagnostics
            .Select(static diagnostic => new DiagnosticSnapshot(
                Id: diagnostic.Id,
                Severity: diagnostic.Severity.ToString(),
                WarningLevel: diagnostic.WarningLevel is 0 ? null : diagnostic.WarningLevel,
                Location: GetLocation(diagnostic.Location),
                Span: GetSpan(diagnostic.Location),
                MessageFormat: diagnostic.Descriptor.MessageFormat.ToString(System.Globalization.CultureInfo.InvariantCulture)))
            .OrderBy(static x => x.Location ?? string.Empty)
            .ThenBy(static x => x.Span ?? string.Empty)
            .ThenBy(static x => x.Id)];
    }

    private static string? GetLocation(Location location) => location.GetLineSpan() switch
    {
        { Path: { Length: > 0 } path } when location.IsInSource => path.Replace('/', '\\'),
        _ => null
    };

    private static string? GetSpan(Location location) => location.GetLineSpan() switch
    {
        var span when location.IsInSource =>
            $"({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1})-({span.EndLinePosition.Line + 1},{span.EndLinePosition.Character + 1})",
        _ => null
    };
}

internal sealed record DiagnosticSnapshot(
    string Id,
    string Severity,
    int? WarningLevel,
    string? Location,
    string? Span,
    string MessageFormat);

internal static class StringExtensions
{
    internal static string ReplaceType(this string source, string from, string to)
    {
        // Require word boundaries so we don't accidentally match substrings.
        // Also avoid adding global:: if it's just 'IBrush' or similar simple replacement,
        // but for framework types, global:: ensures there's no namespace conflict.
        var replacement = to.Contains(".") ? $"global::{to}" : to;
        return Regex.Replace(source, $@"\b{from}\b", replacement);
    }
}


