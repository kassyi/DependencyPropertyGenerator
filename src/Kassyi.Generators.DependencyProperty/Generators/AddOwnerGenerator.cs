using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;


namespace Kassyi.Generators.DependencyProperty.Generators;

[Generator]
public class AddOwnerGenerator : IIncrementalGenerator
{
    #region Constants

    private const string Id = "AOG";

    #endregion

    #region Methods

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource(
                hintName: "AddOwnerAttribute.g.cs",
                source: Resources.AddOwnerAttribute_cs.AsString());
        });

        var framework = context.DetectFramework();
        var version = context.DetectVersion();

        const string ns = "Kassyi.Generators.DependencyProperty.";
        const string attributeName = $"{ns}AddOwnerAttribute";
        var attributes = new[]
        {
            attributeName,
            $"{attributeName}`2"
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
        var (((_, attributes, _, classSymbol), framework), version) = tuple;
        if (framework is not (Framework.Avalonia or Framework.Wpf) ||
            attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var dependencyPropertyData =
            attribute.GetDependencyPropertyData(framework, version, classSymbol, isAddOwner: true);

        return (classData, dependencyPropertyData);
    }

    private static FileWithName GetSourceCode((ClassData Class, DependencyPropertyData DependencyProperty) data)
    {
        var writer = new SourceWriter();
        try
        {
            SourceGenerationHelper.GenerateDependencyProperty(ref writer, data.Class, data.DependencyProperty);
            return new FileWithName(
                Name: $"{data.Class.FullName}.AddOwner.{data.DependencyProperty.Name}.g.cs",
                Text: writer.ToString());
        }
        finally
        {
            writer.Dispose();
        }
    }

    #endregion
}


