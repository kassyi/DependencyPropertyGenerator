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

        const string ns = "Kassyi.Generators.DependencyProperty.";
        const string attributeName = $"{ns}DependencyPropertyAttribute";
        var attributes = new[]
        {
            attributeName,
            $"{attributeName}`1"
        };

        context.RegisterAttributeGenerator(
            framework,
            version,
            attributes,
            PrepareData,
            GetSourceCode,
            Id);
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
        var writer = new SourceWriter();
        try
        {
            SourceGenerationHelper.GenerateDependencyProperty(ref writer, data.Class, data.DependencyProperty);
            return new FileWithName(
                Name: $"{data.Class.FullName}.Properties.{data.DependencyProperty.Name}.g.cs",
                Text: writer.ToString());
        }
        finally
        {
            writer.Dispose();
        }
    }

    #endregion
}


