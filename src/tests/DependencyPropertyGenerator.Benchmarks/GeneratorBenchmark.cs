using BenchmarkDotNet.Attributes;
using H.Generators.Extensions;
using H.Generators.Tests.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace H.Generators.Benchmarks;

[MemoryDiagnoser]
public class GeneratorBenchmark
{
    [Params(Framework.Wpf, Framework.WinUi, Framework.Maui, Framework.Avalonia)]
    public Framework Framework { get; set; }

    private Compilation _compilation = null!;
    private Compilation _modifiedCompilation = null!;
    private GeneratorDriver _driver = null!;

    [GlobalSetup]
    public void Setup()
    {
        var source = GetSourceText(Framework);
        var modifiedSource = source + "\n// Minor comment change to trigger incremental step\n";

        var referenceAssemblies = Framework switch
        {
            Framework.None => ReferenceAssemblies.NetFramework.Net48.Wpf,
            Framework.Wpf => ReferenceAssemblies.NetFramework.Net48.Wpf,
            Framework.Uwp => FrameworkReferenceAssemblies.Net80Uwp,
            Framework.WinUi => FrameworkReferenceAssemblies.Net80WinUi,
            Framework.Uno => FrameworkReferenceAssemblies.Net80Uno,
            Framework.UnoWinUi => FrameworkReferenceAssemblies.Net80UnoWinUi,
            Framework.Avalonia => FrameworkReferenceAssemblies.Net60Avalonia,
            Framework.Maui => FrameworkReferenceAssemblies.Net70Maui,
            _ => throw new NotImplementedException(),
        };

        var references = referenceAssemblies.ResolveAsync(null, CancellationToken.None).GetAwaiter().GetResult();

        _compilation = CSharpCompilation.Create(
            assemblyName: "BenchmarkAssembly",
            syntaxTrees: [
                CSharpSyntaxTree.ParseText(source, options: new CSharpParseOptions(LanguageVersion.Preview))
            ],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        _modifiedCompilation = CSharpCompilation.Create(
            assemblyName: "BenchmarkAssembly",
            syntaxTrees: [
                CSharpSyntaxTree.ParseText(modifiedSource, options: new CSharpParseOptions(LanguageVersion.Preview))
            ],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var globalOptions = GetGlobalOptions(Framework);

        var generators = new IIncrementalGenerator[]
        {
            new DependencyPropertyGenerator(),
            new AttachedDependencyPropertyGenerator(),
            new RoutedEventGenerator(),
            new WeakEventGenerator(),
            new OverrideMetadataGenerator(),
            new AddOwnerGenerator(),
            new StaticConstructorGenerator(),
        };

        _driver = CSharpGeneratorDriver.Create(
            generators: generators.Select(GeneratorExtensions.AsSourceGenerator),
            parseOptions: CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview))
            .WithUpdatedAnalyzerConfigOptions(new DictionaryAnalyzerConfigOptionsProvider(globalOptions));
    }

    [Benchmark(Baseline = true)]
    public GeneratorDriver RunInitialGeneration()
    {
        return _driver.RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);
    }

    [Benchmark]
    public GeneratorDriver RunIncrementalGeneration()
    {
        var driver = _driver.RunGeneratorsAndUpdateCompilation(_compilation, out _, out _);
        return driver.RunGeneratorsAndUpdateCompilation(_modifiedCompilation, out _, out _);
    }

    private static Dictionary<string, string> GetGlobalOptions(Framework framework)
    {
        var globalOptions = new Dictionary<string, string>();
        if (framework == Framework.Wpf) globalOptions.Add("build_property.UseWPF", "true");
        else if (framework == Framework.WinUi) globalOptions.Add("build_property.UseWinUI", "true");
        else if (framework == Framework.Maui) globalOptions.Add("build_property.UseMaui", "true");
        else if (framework == Framework.Uwp) globalOptions.Add("build_property.RecognizeFramework_DefineConstants", "WINDOWS_UWP");
        else if (framework == Framework.Uno) globalOptions.Add("build_property.RecognizeFramework_DefineConstants", "HAS_UNO");
        else if (framework == Framework.UnoWinUi) globalOptions.Add("build_property.RecognizeFramework_DefineConstants", "HAS_UNO;HAS_WINUI");
        else if (framework == Framework.Avalonia) globalOptions.Add("build_property.RecognizeFramework_DefineConstants", "HAS_AVALONIA");

        globalOptions.Add("build_property.RecognizeFramework_Version", "0.0.0.0");
        return globalOptions;
    }

    private static string GetSourceText(Framework framework)
    {
        var usings = framework switch
        {
            Framework.WinUi or Framework.UnoWinUi => "using Microsoft.UI.Xaml;\nusing Microsoft.UI.Xaml.Controls;",
            Framework.Uwp or Framework.Uno => "using Windows.UI.Xaml;\nusing Windows.UI.Xaml.Controls;",
            Framework.Avalonia => "using Avalonia;\nusing Avalonia.Controls;",
            Framework.Maui => "using Microsoft.Maui.Controls;",
            _ => "using System.Windows;\nusing System.Windows.Controls;",
        };

        var controlName = framework == Framework.Maui ? "Grid" : "UserControl";

        return $$"""
            {{usings}}
            using System;
            using System.ComponentModel;
            using DependencyPropertyGenerator;

            #nullable enable

            namespace H.Generators.BenchmarkTests;

            [DependencyProperty<string>("Text", Category = "Behavior", Description = "Text property")]
            [DependencyProperty<bool>("IsSpinning", DefaultValue = true)]
            [DependencyProperty<int>("Count", DefaultValue = 0)]
            [RoutedEvent("Click", RoutedEventStrategy.Bubble)]
            [WeakEvent("Completed")]
            public partial class MyBenchmarkControl : {{controlName}}
            {
                partial void OnTextChanged(string? oldValue, string? newValue) { }
                partial void OnIsSpinningChanged(bool oldValue, bool newValue) { }
            }

            [AttachedDependencyProperty<object, Grid>("SelectedItem", DefaultBindingMode = DefaultBindingMode.TwoWay)]
            [AttachedDependencyProperty<bool, Grid>("IsActivated")]
            public static partial class GridExtensions
            {
            }
            """;
    }
}
