using Kassyi.Generators.Extensions;
using Kassyi.Generators.Extensions.Models;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

internal static class GeneratorHelper
{
    public static void RegisterAttributeGenerator<TData>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<Framework> framework,
        IncrementalValueProvider<string> version,
        IReadOnlyList<string> attributeNames,
        Func<((ClassWithAttributesContext context, Framework framework) left, string version), TData?> prepareData,
        Func<TData, FileWithName> getSourceCode,
        string id,
        bool selectMany = true)
        where TData : struct
    {
        foreach (var attributeName in attributeNames)
        {
            var provider = context.SyntaxProvider
                .ForAttributeWithMetadataNameOfClassesAndRecords(attributeName);

            var combinedProvider = selectMany
                ? provider.SelectManyAllAttributesOfCurrentClassSyntax()
                : provider.SelectAllAttributes();

            combinedProvider
                .Combine(framework)
                .Combine(version)
                .SelectAndReportExceptions(prepareData, context, id)
                .WhereNotNull()
                .SelectAndReportExceptions(getSourceCode, context, id)
                .AddSource(context);
        }
    }

    public static IncrementalValueProvider<EquatableArray<T>> CombineAll<T>(
        this IEnumerable<IncrementalValueProvider<EquatableArray<T>>> providers)
        where T : IEquatable<T>
    {
        var list = providers.ToList();
        if (list.Count == 0)
        {
            throw new ArgumentException("Providers list cannot be empty.", nameof(providers));
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
