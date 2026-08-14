#nullable enable

using System.Runtime.CompilerServices;
using Kassyi.Generators.DependencyProperty.Generators;
using Kassyi.Generators.Tests.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
namespace Kassyi.Generators.DependencyProperty.SnapshotTests;

public abstract class SnapshotTestBase : VerifyBase
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
            values.Select(value => value switch
            {
                "" or null => $"using {prefix};",
                _ when value.StartsWith("System", StringComparison.Ordinal) => $"using {value};",
                _ => $"using {prefix}.{value};"
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

    protected static Dictionary<string, string> GetGlobalOptions(Framework framework)
    {
        var options = new Dictionary<string, string>
        {
            ["build_property.RecognizeFramework_Version"] = "0.0.0.0"
        };

        if (framework switch
        {
            Framework.Wpf => "build_property.UseWPF",
            Framework.WinUi => "build_property.UseWinUI",
            Framework.Maui => "build_property.UseMaui",
            _ => null
        } is { } useProp)
        {
            options[useProp] = "true";
        }
        else if (framework switch
        {
            Framework.Uwp => "WINDOWS_UWP",
            Framework.Uno => "HAS_UNO",
            Framework.UnoWinUi => "HAS_UNO;HAS_WINUI",
            Framework.Avalonia => "HAS_AVALONIA",
            _ => null
        } is { } defines)
        {
            options["build_property.RecognizeFramework_DefineConstants"] = defines;
        }

        return options;
    }

    protected static ReferenceAssemblies GetReferenceAssemblies(Framework framework) => framework switch
    {
        Framework.None or Framework.Wpf => ReferenceAssemblies.NetFramework.Net48.Wpf,
        Framework.Uwp => FrameworkReferenceAssemblies.Net80Uwp,
        Framework.WinUi => FrameworkReferenceAssemblies.Net80WinUi,
        Framework.Uno => FrameworkReferenceAssemblies.Net80Uno,
        Framework.UnoWinUi => FrameworkReferenceAssemblies.Net80UnoWinUi,
        Framework.Avalonia => FrameworkReferenceAssemblies.Net60Avalonia,
        Framework.Maui => FrameworkReferenceAssemblies.Net70Maui,
        _ => throw new NotImplementedException($"Framework {framework} is not supported.")
    };

    private static string ApplyFrameworkReplacements(string source, Framework framework) => framework switch
    {
        Framework.Wpf => source
            .Replace("PointerEntered", "MouseEnter")
            .Replace("PointerExited", "MouseLeave")
            .Replace("PointerRoutedEventArgs", "MouseEventArgs"),

        Framework.Uno or Framework.UnoWinUi or Framework.WinUi or Framework.Uwp => source
            .Replace("KeyEventArgs", "KeyRoutedEventArgs"),

        Framework.Avalonia => source
            .ReplaceType("DispatcherObject", "Avalonia.AvaloniaObject")
            .ReplaceType("DependencyObject", "Avalonia.AvaloniaObject")
            .ReplaceType("Visual", "Avalonia.Interactivity.Interactive")
            .ReplaceType("UIElement", "Avalonia.Input.InputElement")
            .ReplaceType("FrameworkElement", "Avalonia.Controls.Control")
            .Replace("static partial class", "partial class")
            .Replace("Brush", "IBrush")
            .Replace("PointerRoutedEventArgs", "PointerEventArgs"),

        Framework.Maui => $"""
            using Microsoft.Maui.Controls;
            {source
                .Replace("using Microsoft.Maui.Input;", string.Empty)
                .Replace("using Microsoft.Maui.Controls;", string.Empty)
                .Replace("using Microsoft.Maui.Media;", string.Empty)
                .Replace("UIElement", "VisualElement")
                .Replace("FrameworkElement", "VisualElement")
                .Replace("TextBox", "Entry")
                .Replace("UserControl", "Grid")
                .Replace("TreeView", "Grid")
                .Replace("MyControl", "MyGrid")
                .Replace("KeyUp", "SizeChanged")
                .Replace("KeyEventArgs", "global::System.EventArgs")
                .Replace("PointerEntered", "Loaded")
                .Replace("PointerExited", "Unloaded")
                .Replace("PointerRoutedEventArgs", "global::System.EventArgs")}
            """,

        _ => source
    };

    private static async Task<(Compilation Compilation, GeneratorDriver Driver)> CreateCompilationAndDriverAsync<T>(
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
            .WithUpdatedAnalyzerConfigOptions(new DictionaryAnalyzerConfigOptionsProvider(GetGlobalOptions(framework)));

        return (compilation, driver);
    }

    protected async Task CheckSourceAsync<T>(
        string source,
        Framework framework,
        [CallerMemberName] string? callerName = null,
        CancellationToken cancellationToken = default,
        params IIncrementalGenerator[] additionalGenerators)
        where T : IIncrementalGenerator, new()
    {
        var (compilation, driver) = await CreateCompilationAndDriverAsync<T>(source, framework, cancellationToken, additionalGenerators);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _, cancellationToken);
        var diagnostics = compilation.GetDiagnostics(cancellationToken);

        await Task.WhenAll(
            Verify(diagnostics.ToSnapshotModels())
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

    private static string? GetLocation(Location location) => location switch
    {
        { IsInSource: true } when location.GetLineSpan().Path is { Length: > 0 } path => path.Replace('/', '\\'),
        _ => null
    };

    private static string? GetSpan(Location location) => location switch
    {
        { IsInSource: true } when location.GetLineSpan() is var span =>
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
        return source
            .Replace($": {from}", $": global::{to}")
            .Replace($"{from}.", $"global::{to}.")
            .Replace($", {from}", $", global::{to}")
            .Replace($"<{from}", $"<global::{to}")
            .Replace($"{from}>", $"global::{to}>")
            .Replace($"({from}", $"(global::{to}")
            .Replace($"{from})", $"global::{to})");
    }
}


