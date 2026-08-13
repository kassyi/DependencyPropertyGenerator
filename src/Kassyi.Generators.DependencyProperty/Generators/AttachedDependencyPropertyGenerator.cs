using Kassyi.Generators.DependencyProperty.Models;
using Kassyi.Generators.DependencyProperty.Sources;
using Kassyi.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassyi.Generators.DependencyProperty.Generators;

[Generator]
public class AttachedDependencyPropertyGenerator : AttributeGeneratorBase<(ClassData Class, DependencyPropertyData DependencyProperty)>
{
    protected override string Id => "ADPG";

    protected override IReadOnlyList<string> AttributeNames { get; } =
    [
        KnownAttributes.AttachedDependencyProperty,
        $"{KnownAttributes.AttachedDependencyProperty}`1",
        $"{KnownAttributes.AttachedDependencyProperty}`2"
    ];

    protected override void PostInitialize(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            hintName: "AttachedDependencyPropertyAttribute.g.cs",
            source: Resources.AttachedDependencyPropertyAttribute_cs.AsString());
    }

    protected override (ClassData Class, DependencyPropertyData DependencyProperty)? PrepareData(
        ((ClassWithAttributesContext context, Framework framework) left, string version) tuple)
    {
        var (((_, attributes, classSyntax, classSymbol), framework), version) = tuple;
        if (attributes.FirstOrDefault() is not { } attribute)
        {
            return null;
        }

        var classData = classSymbol.GetClassData(framework, version);
        var dependencyPropertyData = attribute.GetDependencyPropertyData(framework, version,
            classSymbol, classSyntax.TryFindAttributeSyntax(attribute), isAttached: true);

        return (classData, dependencyPropertyData);
    }

    protected override CompilationUnitSyntax GenerateSyntax((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        SourceGenerationHelper.GenerateAttachedDependencyPropertySyntax(data.Class, data.DependencyProperty);

    protected override string GetHintName((ClassData Class, DependencyPropertyData DependencyProperty) data) =>
        $"{data.Class.FullName}.AttachedProperties.{data.DependencyProperty.Name}.g.cs";
}
