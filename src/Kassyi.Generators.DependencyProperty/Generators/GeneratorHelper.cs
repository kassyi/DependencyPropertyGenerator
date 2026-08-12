using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

internal static class GeneratorHelper
{
    public static void RegisterAttributeGenerator<TData>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<Framework> framework,
        IncrementalValueProvider<string> version,
        string[] attributeNames,
        Func<((ClassWithAttributesContext context, Framework framework) left, string version), TData?> prepareData,
        Func<TData, FileWithName> getSourceCode,
        string id,
        bool selectMany = true)
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
}
