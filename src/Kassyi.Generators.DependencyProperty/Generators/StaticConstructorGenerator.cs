using System.Collections.Immutable;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for platform static constructor registrations.</summary>
[Generator]
public class StaticConstructorGenerator : IIncrementalGenerator
{
    #region Constants

    private const string Id = "SCG";

    #endregion

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

        var framework = context.DetectFramework();
        var version = context.DetectVersion();



        (string Name, bool IsAttached)[] attributes =
        [
            (KnownAttributes.DependencyProperty, false),
            ($"{KnownAttributes.DependencyProperty}`1", false),
            (KnownAttributes.AttachedDependencyProperty, true),
            ($"{KnownAttributes.AttachedDependencyProperty}`1", true),
            ($"{KnownAttributes.AttachedDependencyProperty}`2", true)
        ];

        var providers = attributes
            .Select(attr => GetClassData(context, attr.Name, framework, version, attr.IsAttached));

        providers.CombineAll()
            .SelectMany(TransformToStaticConstructorData)
            .WithComparer(EqualityComparer<StaticConstructorData>.Default)
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
    }

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
        in GeneratorAttributeContext context,
        bool isAttached)
    {
        var dependencyPropertyData = context.Attribute.GetDependencyPropertyData(
            context.Framework,
            context.Version,
            context.ClassSymbol,
            context.ClassSyntax.TryFindAttributeSyntax(context.Attribute),
            isAttached: isAttached,
            semanticModel: context.SemanticModel);

        return (context.ClassData, dependencyPropertyData);
    }

    private static FileWithName GetSourceCode(StaticConstructorData data)
    {
        var writer = new SourceWriter();
        try
        {
            SourceGenerationHelper.GenerateStaticConstructor(
                ref writer,
                data.Class,
                [.. data.Properties.Where(static property => !property.IsDirect)]);
            var text = writer.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                return FileWithName.Empty;
            }

            return new FileWithName(Name: $"{data.Class.FullName}.StaticConstructor.g.cs", Text: text);
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
        return context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords(attributeName)
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndCatchExceptions(x =>
            {
                var (((semanticModel, attributes, classSyntax, classSymbol), frameworkVal), versionVal) = x;
                if (attributes.IsEmpty)
                {
                    return default((ClassData, DependencyPropertyData)?);
                }

                var classData = classSymbol.GetClassData(frameworkVal, versionVal);
                var ctx = new GeneratorAttributeContext(
                    semanticModel,
                    attributes[0],
                    classSyntax,
                    classSymbol,
                    frameworkVal,
                    versionVal,
                    classData);
                return PrepareData(ctx, isAttached: isAttached);
            })
            .WhereNotNull()
            .CollectAsEquatableArray();
    }
    #endregion
}
