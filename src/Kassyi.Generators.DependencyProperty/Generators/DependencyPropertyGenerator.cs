using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;


namespace Kassyi.Generators.DependencyProperty.Generators;

[Generator]
public class DependencyPropertyGenerator : IIncrementalGenerator
{
    #region Constants

    private const string Id = "DPG";

    #endregion

    #region Methods

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                hintName: "DependencyPropertyAttribute.g.cs",
                source: Resources.DependencyPropertyAttribute_cs.AsString());
        });

        var framework = context.DetectFramework();
        var version = context.DetectVersion();

        context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords(
                "Kassyi.Generators.DependencyProperty.DependencyPropertyAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(PrepareData, context, Id)
            .WhereNotNull()
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
        context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords(
                "Kassyi.Generators.DependencyProperty.DependencyPropertyAttribute`1")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(PrepareData, context, Id)
            .WhereNotNull()
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
    }

    private static (ClassData Class, DependencyPropertyData DependencyProperty)? PrepareData(
        ((ClassWithAttributesContext context,
            Framework framework) left,
            string version) tuple)
    {
        var (((_, attributes, classSyntax, classSymbol), framework), version) = tuple;
        if (attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var dependencyPropertyData =
            attribute.GetDependencyPropertyData(framework, version, classSymbol,
                classSyntax.TryFindAttributeSyntax(attribute));

        return (classData, dependencyPropertyData);
    }

    private static FileWithName GetSourceCode((ClassData Class, DependencyPropertyData DependencyProperty) data)
    {
        return new FileWithName(
            Name: $"{data.Class.FullName}.Properties.{data.DependencyProperty.Name}.g.cs",
            Text: SourceGenerationHelper.GenerateDependencyProperty(data.Class, data.DependencyProperty));
    }

    #endregion
}


