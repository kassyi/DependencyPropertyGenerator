using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Core assembly-internal helper for extracting syntax data and registering attribute-based generators.</summary>
internal static class GeneratorHelper
{
    /// <summary>Extracts framework-specific metadata from attributes and prepares it for incremental generation.</summary>
    public static IncrementalValuesProvider<TData> ExtractData<TData>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<Framework> framework,
        IncrementalValueProvider<string> version,
        string attributeName,
        Func<GeneratorMultiAttributeContext, TData?> prepareData,
        string id,
        bool selectMany = true,
        bool reportExceptions = true)
        where TData : struct
    {
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords(attributeName);

        // [WHY] SelectMany flattens multiple attributes applied to the same class or partial declarations into individual elements.
        var combinedProvider = selectMany
            ? provider.SelectManyAllAttributesOfCurrentClassSyntax()
            : provider.SelectAllAttributes();

        var combined = combinedProvider
            .Combine(framework)
            .Combine(version);

        if (reportExceptions)
        {
            return combined.SelectAndReportExceptions(x =>
            {
                var (((semanticModel, attributes, classSyntax, classSymbol), frameworkVal), versionVal) = x;
                if (attributes.IsEmpty)
                {
                    return null;
                }

                var classData = classSymbol.GetClassData(frameworkVal, versionVal);
                var ctx = new GeneratorMultiAttributeContext(
                    semanticModel,
                    attributes,
                    classSyntax,
                    classSymbol,
                    frameworkVal,
                    versionVal,
                    classData);
                return prepareData(ctx);
            }, context, id).WhereNotNull();
        }
        else
        {
            return combined.SelectAndCatchExceptions(x =>
            {
                var (((semanticModel, attributes, classSyntax, classSymbol), frameworkVal), versionVal) = x;
                if (attributes.IsEmpty)
                {
                    return null;
                }

                var classData = classSymbol.GetClassData(frameworkVal, versionVal);
                var ctx = new GeneratorMultiAttributeContext(
                    semanticModel,
                    attributes,
                    classSyntax,
                    classSymbol,
                    frameworkVal,
                    versionVal,
                    classData);
                return prepareData(ctx);
            }).WhereNotNull();
        }
    }

    /// <summary>Registers source generation pipelines for multiple attribute names sharing the same extraction logic.</summary>
    public static void RegisterAttributeGenerator<TData>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<Framework> framework,
        IncrementalValueProvider<string> version,
        IReadOnlyList<string> attributeNames,
        Func<GeneratorMultiAttributeContext, TData?> prepareData,
        Func<TData, FileWithName> getSourceCode,
        string id,
        bool selectMany = true)
        where TData : struct
    {
        foreach (var attributeName in attributeNames)
        {
            context.ExtractData(framework, version, attributeName, prepareData, id, selectMany)
                .SelectAndReportExceptions(getSourceCode, context, id)
                .AddSource(context);
        }
    }

    /// <summary>Combines multiple incremental value providers into a single unified array provider.</summary>
    public static IncrementalValueProvider<EquatableArray<T>> CombineAll<T>(
        this IEnumerable<IncrementalValueProvider<EquatableArray<T>>> providers,
        IncrementalGeneratorInitializationContext context)
        where T : IEquatable<T>
    {
        var list = providers.ToList();
        if (list.Count == 0)
        {
            return context.AnalyzerConfigOptionsProvider.Select((_, _) => EquatableArray<T>.Empty);
        }

        var combined = list[0];
        for (var i = 1; i < list.Count; i++)
        {
            combined = combined
                .Combine(list[i])
                .Select(static (x, _) => x.Left.AsImmutableArray().AddRange(x.Right.AsImmutableArray()).AsEquatableArray());
        }

        return combined;
    }
}
