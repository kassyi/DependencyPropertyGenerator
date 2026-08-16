using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Kassyi.Generators.DependencyProperty.Generators;

/// <summary>Incremental generator for standard dependency properties.</summary>
[Generator]
public class DependencyPropertyGenerator : AttributeGeneratorBase<(ClassData Class, DependencyPropertyData DependencyProperty)>
{
    protected override string Id => "DPG";

    protected override IReadOnlyList<string> AttributeNames { get; } =
    [
        KnownAttributes.DependencyProperty,
        $"{KnownAttributes.DependencyProperty}`1"
    ];

    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "DependencyPropertyAttribute.g.cs",
            source: Resources.DependencyPropertyAttribute_cs.AsString());
    }

    protected override (ClassData Class, DependencyPropertyData DependencyProperty)? PrepareData(
        ((ClassWithAttributesContext context, Framework framework) left, string version) tuple)
    {
        var (((semanticModel, attributes, classSyntax, classSymbol), framework), version) = tuple;
        if (attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var dependencyPropertyData =
            attribute.GetDependencyPropertyData(framework, version, classSymbol,
                classSyntax.TryFindAttributeSyntax(attribute), semanticModel: semanticModel);

        return (classData, dependencyPropertyData);
    }

    protected override string GenerateSource((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        SourceGenerationHelper.GenerateDependencyPropertySource(data.Class, data.DependencyProperty);

    protected override string GetHintName((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        $"{data.Class.FullName}.Properties.{data.DependencyProperty.Name.SanitizeFileName()}.g.cs";
}
