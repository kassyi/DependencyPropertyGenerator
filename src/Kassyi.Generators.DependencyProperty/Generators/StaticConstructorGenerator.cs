using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Diagnostics;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for platform static constructor registrations.</summary>
[Generator]
public class StaticConstructorGenerator : IIncrementalGenerator
{
    #region Methods

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                hintName: "Localizability.g.cs",
                source: Resources.Localizability_cs.AsString());
            context.AddSource(
                hintName: "DefaultBindingMode.g.cs",
                source: Resources.DefaultBindingMode_cs.AsString());
            context.AddSource(
                hintName: "SourceTrigger.g.cs",
                source: Resources.SourceTrigger_cs.AsString());
        });

        var framework = context.DetectFramework(DiagnosticDescriptors.FrameworkNotRecognized);
        var version = context.DetectVersion();

        IncrementalValueProvider<EquatableArray<(ClassData Class, DependencyPropertyData DependencyProperty)>>[] providers =
        [
            GetClassData(context, KnownAttributes.DependencyProperty, framework, version, isAttached: false),
            GetClassData(context, $"{KnownAttributes.DependencyProperty}`1", framework, version, isAttached: false),
            GetClassData(context, KnownAttributes.AttachedDependencyProperty, framework, version, isAttached: true),
            GetClassData(context, $"{KnownAttributes.AttachedDependencyProperty}`1", framework, version, isAttached: true),
            GetClassData(context, $"{KnownAttributes.AttachedDependencyProperty}`2", framework, version, isAttached: true),
        ];

        providers.CombineAll(context)
            .SelectMany(TransformToStaticConstructorData)
            .WithComparer(EqualityComparer<StaticConstructorData>.Default)
            .SelectAndReportExceptions(GetSourceCode, context, DiagnosticDescriptors.UnhandledExceptionId)
            .AddSource(context);
    }

    /// <summary>Groups flat dependency property data by declaring class for static constructor generation.</summary>
    internal static IEnumerable<StaticConstructorData> TransformToStaticConstructorData(
        EquatableArray<(ClassData Class, DependencyPropertyData DependencyProperty)> array,
        CancellationToken _)
    {
        var dictionary = new Dictionary<ClassData, ImmutableArray<DependencyPropertyData>.Builder>();
        
        foreach (var item in array)
        {
            if (item.Class.Framework != Framework.Avalonia)
            {
                continue;
            }

            if (!dictionary.TryGetValue(item.Class, out var builder))
            {
                builder = ImmutableArray.CreateBuilder<DependencyPropertyData>();
                dictionary.Add(item.Class, builder);
            }
            builder.Add(item.DependencyProperty);
        }

        var result = new List<StaticConstructorData>(dictionary.Count);
        foreach (var kvp in dictionary)
        {
            result.Add(new StaticConstructorData(
                Class: kvp.Key,
                Properties: kvp.Value.ToImmutable().AsEquatableArray()));
        }

        return result;
    }

    private static (ClassData Class, DependencyPropertyData DependencyProperty)? PrepareData(
        GeneratorAttributeContext context,bool isAttached) =>
        (context.ClassData, context.GetDependencyPropertyData(isAttached: isAttached));

    private static FileWithName GetSourceCode(StaticConstructorData data)
    {
        var writer = new SourceWriter();
        try
        {
            SourceGenerationHelper.GenerateStaticConstructor(
                ref writer,
                data.Class,
                [.. data.Properties.Where(static property => !property.Modifiers.IsDirect)]);
            var text = writer.ToString();
            return string.IsNullOrWhiteSpace(text) ? FileWithName.Empty 
                : new FileWithName(Name: $"{data.Class.FullName}.StaticConstructor.g.cs", Text: text);
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static IncrementalValueProvider<EquatableArray<(ClassData Class, DependencyPropertyData DependencyProperty)>> GetClassData(
        IncrementalGeneratorInitializationContext context,
        string attributeName,
        IncrementalValueProvider<Framework> framework,
        IncrementalValueProvider<string> version,
        bool isAttached)
    {
        return context.ExtractData(
                framework,
                version,
                attributeName,
                ctx => PrepareData(ctx.ForFirstAttribute(), isAttached),
                selectMany: true,
                reportExceptions: false)
            .CollectAsEquatableArray();
    }
    #endregion
}
