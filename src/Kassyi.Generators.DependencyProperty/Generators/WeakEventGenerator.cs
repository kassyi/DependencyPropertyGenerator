using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;



namespace Kassyi.Generators.DependencyProperty.Generators;

[Generator]
public class WeakEventGenerator : IIncrementalGenerator
{
    #region Constants

    private const string Id = "WEG";

    #endregion

    #region Methods

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                hintName: "WeakEventAttribute.g.cs",
                source: Resources.WeakEventAttribute_cs.AsString());
        });

        var framework = context.DetectFramework();
        var version = context.DetectVersion();

        context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords("Kassyi.Generators.DependencyProperty.WeakEventAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(PrepareData, context, Id)
            .WhereNotNull()
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
        context.SyntaxProvider
            .ForAttributeWithMetadataNameOfClassesAndRecords("Kassyi.Generators.DependencyProperty.WeakEventAttribute`1")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .Combine(framework)
            .Combine(version)
            .SelectAndReportExceptions(PrepareData, context, Id)
            .WhereNotNull()
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
    }

    private static (ClassData Class, EventData Event)? PrepareData(
        ((ClassWithAttributesContext context,
            Framework framework) left,
            string version) tuple)
    {
        var (((_, attributes, _, classSymbol), framework), version) = tuple;
        if (framework is not (Framework.Maui or Framework.Wpf) ||
            attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var eventData = attribute.GetEventData(isStaticClass: classData.IsStatic);

        return (classData, eventData);
    }

    private static FileWithName GetSourceCode((ClassData Class, EventData Event) data)
    {
        return new FileWithName(
            Name: $"{data.Class.FullName}.WeakEvents.{data.Event.Name}.g.cs",
            Text: SourceGenerationHelper.GenerateWeakEvent(data.Class, data.Event));
    }

    #endregion
}


